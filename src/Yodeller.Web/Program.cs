using MediatR;
using Yodeller.Application;
using Yodeller.Application.Ports;
using Yodeller.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(typeof(ApplicationLayerDependencies).Assembly);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<IClock, QuickAndDirtyClock>();

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

internal class QuickAndDirtyClock : IClock
{
    public DateTime GetNow() => DateTime.Now;
}
