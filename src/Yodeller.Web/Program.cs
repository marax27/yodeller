using MediatR;
using Yodeller.Application;
using Yodeller.Application.Messages;
using Yodeller.Application.Ports;
using Yodeller.Infrastructure.Adapters;
using Yodeller.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(typeof(ApplicationLayerDependencies).Assembly);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IClock, BasicClock>();
builder.Services.AddSingleton<IMessageProducer<BaseMessage>, InMemoryMessageProducer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseMiddleware<CustomExceptionHandlingMiddleware>();

app.MapControllers();

app.Run();

public partial class Program { }
