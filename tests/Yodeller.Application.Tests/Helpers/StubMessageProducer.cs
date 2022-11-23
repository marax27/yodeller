using Core.Shared.StateManagement;
using Yodeller.Application.Features.RequestDownload;
using Yodeller.Application.Models;
using Yodeller.Application.Ports;
using Yodeller.Application.State;

namespace Yodeller.Application.Tests.Helpers;

internal class StubMessageProducer : IMessageProducer<IStateReducer<DownloadRequestsState>>
{
    private readonly List<IStateReducer<DownloadRequestsState>> _downloadRequests = new();

    public IEnumerable<DownloadRequest> GetAll() => throw new NotImplementedException();

    public IReadOnlyList<DownloadRequest> GetRegisteredDownloadRequests() => _downloadRequests
        .OfType<AddNewRequestReducer>()
        .Select(message => message.NewRequest)
        .ToArray();

    public void Produce(IStateReducer<DownloadRequestsState> reducer) => _downloadRequests.Add(reducer);
}
