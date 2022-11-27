using System.Diagnostics;
using Microsoft.AspNetCore.SignalR;
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

    public async Task SendStatusChange(string requestId, DownloadRequestStatus newStatus)
    {
        await _hubContext.Clients.All.SendAsync("StatusChange", requestId, MapStatus(newStatus));
    }

    private static string MapStatus(DownloadRequestStatus status) => status switch
    {
        DownloadRequestStatus.New => "New",
        DownloadRequestStatus.InProgress => "In progress",
        DownloadRequestStatus.Completed => "Completed",
        DownloadRequestStatus.Failed => "Failed",
        DownloadRequestStatus.Cancelled => "Cancelled",
        _ => throw new UnreachableException(nameof(status))
    };
}
