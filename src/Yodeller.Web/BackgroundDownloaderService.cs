using Yodeller.Application.Downloader;

namespace Yodeller.Web;

public class BackgroundDownloaderService : BackgroundService
{
    private readonly MediaDownloadScheduler _job;
    private readonly ILogger<BackgroundDownloaderService> _logger;

    public BackgroundDownloaderService(MediaDownloadScheduler job, ILogger<BackgroundDownloaderService> logger)
    {
        _job = job;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Starting downloader loop in the background.");
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _job.Execute(stoppingToken);
                await Task.Delay(103, stoppingToken);
            }
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Unhandled error in the downloader loop: {ErrorMessage}", e.Message);
            throw;
        }
        _logger.LogInformation("Downloader loop finished.");
    }
}
