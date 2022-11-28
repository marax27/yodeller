using Core.Shared.StateManagement;
using Yodeller.Application.Models;
using Yodeller.Application.State;

namespace Yodeller.Application.Features.RequestDownload;

public record AddNewRequestReducer(DownloadRequest NewRequest) : IStateReducer<DownloadRequestsState>
{
    public DownloadRequestsState Invoke(DownloadRequestsState oldState)
    {
        var requests = oldState.Requests.Append(NewRequest).ToList();

        return oldState with { Requests = requests };
    }
}
