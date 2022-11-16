using Microsoft.Extensions.Logging;
using Yodeller.Application.Messages;
using Yodeller.Application.Models;
using Yodeller.Application.Ports;

namespace Yodeller.Application.Downloader;

public class MediaDownloadScheduler
{
    private readonly IMediaDownloader _downloader;
    private readonly IMessageConsumer<BaseMessage> _messageConsumer;
    private readonly IDownloadRequestsRepository _requestsRepository;
    private readonly ILogger<MediaDownloadScheduler> _logger;

    private Task? _downloadInProgress = null;

    public MediaDownloadScheduler(
        IMediaDownloader downloader,
        IMessageConsumer<BaseMessage> messageConsumer,
        IDownloadRequestsRepository requestsRepository,
        ILogger<MediaDownloadScheduler> logger)
    {
        _downloader = downloader ?? throw new ArgumentNullException(nameof(downloader));
        _messageConsumer = messageConsumer ?? throw new ArgumentNullException(nameof(messageConsumer));
        _requestsRepository = requestsRepository ?? throw new ArgumentNullException(nameof(requestsRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void Execute()
    {
        ConsumeAllMessages();

        if (_downloadInProgress is null or { IsCompleted: true })
        {
            RunNewDownloadTask();
        }
    }

    private void RunNewDownloadTask()
    {
        _downloadInProgress?.Dispose();

        var request = FindEligibleDownloadRequest();
        if (request is { })
        {
            _requestsRepository.UpdateStatus(request.Id, DownloadRequestStatus.InProgress);
            _downloadInProgress = Task.Run(() => PerformDownload(request));
        }
    }

    private void PerformDownload(DownloadRequest request)
    {
        var downloadSuccessful = false;

        try
        {
            _logger.LogInformation("Starting download of '{MediaLocator}'...", request.MediaLocator);

            var specs = new DownloadProcessSpecification(request.SubtitlePatterns, request.MediaLocator, request.AudioOnly);

            downloadSuccessful = _downloader.Download(specs);

            _logger.LogInformation("Medium '{MediaLocator}' {Message}.",
                request.MediaLocator,
                downloadSuccessful ? "downloaded successfully." : "failed to download.");
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Error while trying to download a medium '{MediaLocator}'.", request.MediaLocator);
        }
        finally
        {
            var newStatus = downloadSuccessful
                ? DownloadRequestStatus.Completed
                : DownloadRequestStatus.Failed;
            _requestsRepository.UpdateStatus(request.Id, newStatus);
        }
    }

    private DownloadRequest? FindEligibleDownloadRequest()
    {
        return _requestsRepository
            .FindByStatus(DownloadRequestStatus.New)
            .FirstOrDefault();
    }

    private void ConsumeAllMessages()
    {
        var processedMessageCount = 0;

        while (_messageConsumer.TryConsume(out var message))
        {
            _logger.LogInformation("Evaluating a message: {MessageType}...", message.GetType().Name);
            message.Invoke(_requestsRepository);
            ++processedMessageCount;
        }

        if (processedMessageCount > 0)
        {
            _logger.LogInformation("Evaluated {MessageCount} message(s).", processedMessageCount);
        }
    }
}
