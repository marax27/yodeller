using MediatR;
using Yodeller.Application;
using Yodeller.Application.Downloader;
using Yodeller.Application.Messages;
using Yodeller.Application.Ports;
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

var messageQueue = new InMemoryMessageQueue();

builder.Services.AddSingleton<IMessageProducer<BaseMessage>>(messageQueue);
builder.Services.AddSingleton<IMessageConsumer<BaseMessage>>(messageQueue);
builder.Services.AddSingleton<IDownloadRequestsRepository>(new DownloadRequestsRepository());

builder.Services.AddHostedService<BackgroundDownloaderService>();

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
