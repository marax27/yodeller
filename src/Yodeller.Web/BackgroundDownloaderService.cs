using Yodeller.Application.Downloader;

namespace Yodeller.Web;

public class BackgroundDownloaderService : BackgroundService
{
    private readonly MediaDownloaderJob _job;

    public BackgroundDownloaderService(MediaDownloaderJob job) => _job = job;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _job.Execute();
            await Task.Delay(1000, stoppingToken);
        }
    }
}
