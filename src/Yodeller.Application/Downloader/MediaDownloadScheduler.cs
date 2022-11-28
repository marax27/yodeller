using Core.Shared.StateManagement;
using Microsoft.Extensions.Logging;
using Yodeller.Application.Downloader.Reducers;
using Yodeller.Application.Models;
using Yodeller.Application.Ports;
using Yodeller.Application.State;

namespace Yodeller.Application.Downloader;

public class MediaDownloadScheduler : IDisposable
{
    private readonly IMediaDownloader _downloader;
    private readonly IMessageProducer<IStateReducer<DownloadRequestsState>> _messageProducer;
    private readonly IClock _clock;
    private readonly ILogger<MediaDownloadScheduler> _logger;

    private Task? _downloadInProgress = null;

    public MediaDownloadScheduler(
        IMediaDownloader downloader,
        IMessageProducer<IStateReducer<DownloadRequestsState>> messageProducer,
        IClock clock,
        ILogger<MediaDownloadScheduler> logger)
    {
        _downloader = downloader ?? throw new ArgumentNullException(nameof(downloader));
        _messageProducer = messageProducer ?? throw new ArgumentNullException(nameof(messageProducer));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Execute(CancellationToken stoppingToken)
    {
        if (_downloadInProgress is null or { IsCompleted: true })
        {
            await RunNewDownloadTask(stoppingToken);
        }
    }

    private async Task RunNewDownloadTask(CancellationToken stoppingToken)
    {
        _downloadInProgress?.Dispose();

        var tcs = new TaskCompletionSource<DownloadRequest?>();
        _messageProducer.Produce(new LockRequestForDownloadReducer(_clock.GetNow(), tcs.SetResult));
        var request = await tcs.Task;

        if (request is { })
        {
            _downloadInProgress = Task.Run(async () => await PerformDownload(request), stoppingToken);
        }
    }

    private async Task<bool> PerformDownload(DownloadRequest request)
    {
        var downloadSuccessful = false;

        try
        {
            _logger.LogInformation("Starting download of '{MediaLocator}'...", request.MediaLocator);

            var specs = new DownloadProcessSpecification(request.SubtitlePatterns, request.MediaLocator, request.AudioOnly);

            downloadSuccessful = await _downloader.Download(specs);

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
            _messageProducer.Produce(new FinishDownloadReducer(request.Id, downloadSuccessful, _clock.GetNow()));
        }

        return downloadSuccessful;
    }

    public void Dispose()
    {
        _downloadInProgress?.Dispose();
    }
}
