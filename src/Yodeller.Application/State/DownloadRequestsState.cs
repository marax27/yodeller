using Yodeller.Application.Models;

namespace Yodeller.Application.State;

public record DownloadRequestsState(
    List<DownloadRequest> Requests,
    List<string> AlteredRequestIds
);
