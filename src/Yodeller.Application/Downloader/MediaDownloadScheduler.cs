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
        var request = FindEligibleDownloadRequest();
        TryDownload(request);
    }

    private void TryDownload(DownloadRequest? request)
    {
        if (request is null) return;

        _requestsRepository.UpdateStatus(request.Id, DownloadRequestStatus.InProgress);

        try
        {
            _logger.LogInformation("Starting download of '{MediaLocator}'...", request.MediaLocator);

            _downloader.Download(request.MediaLocator);

            _logger.LogInformation("Medium '{MediaLocator}' downloaded successfully.", request.MediaLocator);
            _requestsRepository.UpdateStatus(request.Id, DownloadRequestStatus.Completed);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "Failed to download a medium '{MediaLocator}'.", request.MediaLocator);
            _requestsRepository.UpdateStatus(request.Id, DownloadRequestStatus.Failed);
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
