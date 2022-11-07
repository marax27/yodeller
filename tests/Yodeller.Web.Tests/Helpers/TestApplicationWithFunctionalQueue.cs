using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Yodeller.Application.Downloader;
using Yodeller.Application.Ports;

namespace Yodeller.Web.Tests.Helpers;

public class TestApplicationWithFunctionalQueue : WebApplicationFactory<Program>
{
    private readonly List<DownloadProcessSpecification> _executedDownloads = new();

    public IReadOnlyCollection<DownloadProcessSpecification> ExecutedDownloads => _executedDownloads;

    public Mock<IMediaDownloader> MockMediaDownloader { get; } = new();

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(RegisterTestServices);
        return base.CreateHost(builder);
    }

    /// Populate with further services as needed.
    private void RegisterTestServices(IServiceCollection services)
    {
        MockMediaDownloader
            .Setup(mock => mock.Download(It.IsAny<DownloadProcessSpecification>()))
            .Returns(OnDownloadRequest);
        services.AddSingleton(MockMediaDownloader.Object);
    }

    private bool OnDownloadRequest(DownloadProcessSpecification what)
    {
        _executedDownloads.Add(what);
        return true;
    }
}

