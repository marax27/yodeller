using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Yodeller.Application.Ports;

namespace Yodeller.Web.Tests.Helpers;

public class TestApplication : WebApplicationFactory<Program>
{
    public Mock<IRequestRepository> MockRequestRepository { get; } = new();

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.ConfigureServices(RegisterTestServices);
        return base.CreateHost(builder);
    }

    /// Populate with further services as needed.
    private void RegisterTestServices(IServiceCollection services)
    {
        services.AddSingleton(MockRequestRepository.Object);
    }
}

