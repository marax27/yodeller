using Yodeller.Application.Models;
using Yodeller.Application.Ports;

namespace Yodeller.Application.Tests.Helpers;

internal class StubRequestRepository : IRequestRepository
{
    private readonly List<DownloadRequest> _downloadRequests = new();

    public IEnumerable<DownloadRequest> GetAll() => throw new NotImplementedException();

    public void Add(DownloadRequest request) => _downloadRequests.Add(request);

    public IReadOnlyList<DownloadRequest> GetRegisteredDownloadRequests() => _downloadRequests;
}
