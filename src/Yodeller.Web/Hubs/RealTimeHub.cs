using Microsoft.AspNetCore.SignalR;

namespace Yodeller.Web.Hubs;

public class RealTimeHub : Hub
{
    private readonly ILogger<RealTimeHub> _logger;

    public RealTimeHub(ILogger<RealTimeHub> logger)
    {
        _logger = logger;
    }

    public async Task SendMessage(string user, string message)
    {
        var id = Guid.NewGuid().ToString();
        _logger.LogInformation("Processing the Hub message: {Message} @ {User}", message, user);
        await Clients.All.SendAsync("Receive", user, $"{message}-{id}");
    }
}
