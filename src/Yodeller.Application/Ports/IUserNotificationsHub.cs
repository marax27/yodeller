using Yodeller.Application.Models;

namespace Yodeller.Application.Ports;

public interface IUserNotificationsHub
{
    Task SendRequestUpdate(IReadOnlyCollection<DownloadRequest> updatedRequests);
}
