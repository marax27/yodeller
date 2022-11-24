using Core.Shared.StateManagement;
using MediatR;
using Yodeller.Application;
using Yodeller.Application.Downloader;
using Yodeller.Application.Ports;
using Yodeller.Application.State;
using Yodeller.Infrastructure.Adapters;
using Yodeller.Web;
using Yodeller.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(typeof(ApplicationLayerDependencies).Assembly);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IClock, BasicClock>();
builder.Services.AddTransient<MediaDownloadScheduler>();
builder.Services.AddTransient<IMediaDownloader, YtDlpMediaDownloader>();

builder.Services.AddTransient<IDiskSpaceEnvironmentCheck, DiskSpaceEnvironmentCheck>();
builder.Services.AddTransient<IApplicationAvailableEnvironmentCheck, ApplicationAvailableEnvironmentCheck>();

var messageQueue = new InMemoryReducerQueue();

builder.Services.AddSingleton<IMessageProducer<IStateReducer<DownloadRequestsState>>>(messageQueue);
builder.Services.AddSingleton<IMessageConsumer<IStateReducer<DownloadRequestsState>>>(messageQueue);

builder.Services.AddHostedService<BackgroundDownloaderService>();
builder.Services.AddHostedService<BackgroundStateManagementService>(s =>
{
    var initialState = new DownloadRequestsState(new());
    return new(
        new DownloadRequestsStateManager(
            initialState,
            s.GetRequiredService<IMessageConsumer<IStateReducer<DownloadRequestsState>>>(),
            s.GetRequiredService<IClock>(),
            s.GetRequiredService<ILogger<DownloadRequestsStateManager>>()
        ),
        s.GetRequiredService<ILogger<BackgroundStateManagementService>>()
    );
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseMiddleware<CustomExceptionHandlingMiddleware>();

app.MapControllers();

app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();

public partial class Program { }
