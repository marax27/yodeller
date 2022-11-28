using Core.Shared.StateManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Yodeller.Application.Downloader;
using Yodeller.Application.Ports;
using Yodeller.Application.State;

namespace Yodeller.Web.Tests.Helpers;

public class TestApplicationWithMockedQueue : WebApplicationFactory<Program>
{
    public Mock<IMessageProducer<IStateReducer<DownloadRequestsState>>> MockRequestProducer { get; } = new();

    public Mock<IMediaDownloader> MockMediaDownloader { get; } = new();

    public TestApplicationWithMockedQueue()
    {
        MockRequestProducer
            .Setup(producer => producer.Produce(It.IsAny<IStateReducer<DownloadRequestsState>>()))
            .Callback(OnReducerProduced);
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(RegisterTestServices);
        return base.CreateHost(builder);
    }

    /// Populate with further services as needed.
    private void RegisterTestServices(IServiceCollection services)
    {
        services.AddSingleton(MockRequestProducer.Object);

        MockMediaDownloader
            .Setup(mock => mock.Download(It.IsAny<DownloadProcessSpecification>()))
            .ReturnsAsync(true);
        services.AddSingleton(MockMediaDownloader.Object);
    }

    private static void OnReducerProduced(IStateReducer<DownloadRequestsState> reducer)
    {
        var dumbState = new DownloadRequestsState(new(), new());
        reducer.Invoke(dumbState);
    }
}
