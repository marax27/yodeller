using Yodeller.Application.Models;

namespace Yodeller.Application.Ports;

public interface IRequestRepository
{
    void Add(DownloadRequest request);
}
