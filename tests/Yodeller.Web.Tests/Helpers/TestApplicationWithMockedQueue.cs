﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Yodeller.Application.Messages;
using Yodeller.Application.Ports;

namespace Yodeller.Web.Tests.Helpers;

public class TestApplicationWithMockedQueue : WebApplicationFactory<Program>
{
    public Mock<IMessageProducer<BaseMessage>> MockRequestProducer { get; } = new();

    public Mock<IMediaDownloader> MockMediaDownloader { get; } = new();

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
            .Setup(mock => mock.Download(It.IsAny<string>()))
            .Returns(true);
        services.AddSingleton(MockMediaDownloader.Object);
    }
}
