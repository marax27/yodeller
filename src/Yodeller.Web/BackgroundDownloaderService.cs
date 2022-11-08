using Yodeller.Application.Downloader;

namespace Yodeller.Web;

public class BackgroundDownloaderService : BackgroundService
{
    private readonly MediaDownloadScheduler _job;

    public BackgroundDownloaderService(MediaDownloadScheduler job) => _job = job;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _job.Execute();
            await Task.Delay(200, stoppingToken);
        }
    }
}
