using Yodeller.Application.Models;

namespace Yodeller.Application.Ports;

public interface IRequestRepository
{
    IEnumerable<DownloadRequest> GetAll();

    void Add(DownloadRequest request);
}
