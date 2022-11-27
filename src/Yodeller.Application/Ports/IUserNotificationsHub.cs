using Yodeller.Application.Models;

namespace Yodeller.Application.Ports;

public interface IUserNotificationsHub
{
    Task SendStatusChange(string requestId, DownloadRequestStatus newStatus);
}
