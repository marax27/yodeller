using Microsoft.AspNetCore.SignalR;
using Yodeller.Application;
using Yodeller.Application.Models;
using Yodeller.Application.Ports;

namespace Yodeller.Web.Hubs;

public class UserNotificationsHub : IUserNotificationsHub
{
    private readonly IHubContext<RealTimeHub> _hubContext;

    public UserNotificationsHub(IHubContext<RealTimeHub> hubContext)
    {
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
    }

    public async Task SendRequestUpdate(IReadOnlyCollection<DownloadRequest> updatedRequests)
    {
        var result = updatedRequests
            .Select(DownloadRequestMapper.Map)
            .ToArray();

        await _hubContext.Clients.All.SendAsync("RequestUpdate", result);
    }
}
