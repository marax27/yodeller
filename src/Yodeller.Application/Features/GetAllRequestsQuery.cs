using MediatR;
using Yodeller.Application.Models;

namespace Yodeller.Application.Features;

public record GetAllRequestsQuery : IRequest<GetAllRequestsQuery.Result>
{
    public record DownloadRequestDto(
        string Id,
        string MediaLocator,
        bool AudioOnly,
        string Status
    );

    public record Result(
        IReadOnlyCollection<DownloadRequestDto> Requests
    );
}

public class GetAllRequestsQueryHandler : IRequestHandler<GetAllRequestsQuery, GetAllRequestsQuery.Result>
{
    private readonly IDownloadRequestsRepository _requestsRepository;

    public GetAllRequestsQueryHandler(IDownloadRequestsRepository requestsRepository)
    {
        _requestsRepository = requestsRepository ?? throw new ArgumentNullException(nameof( requestsRepository));
    }

    public Task<GetAllRequestsQuery.Result> Handle(GetAllRequestsQuery request, CancellationToken cancellationToken)
    {
        var statuses = new[]
        {
            DownloadRequestStatus.InProgress,
            DownloadRequestStatus.Failed,
            DownloadRequestStatus.New,
            DownloadRequestStatus.Completed,
            DownloadRequestStatus.Cancelled
        };

        var requests = statuses
            .SelectMany(status => _requestsRepository.FindByStatus(status))
            .Select(Map)
            .ToArray();

        return Task.FromResult(new GetAllRequestsQuery.Result(requests));
    }

    private static GetAllRequestsQuery.DownloadRequestDto Map(DownloadRequest model) => new(
        model.Id,
        model.MediaLocator,
        model.AudioOnly,
        model.Status switch
        {
            DownloadRequestStatus.New => "New",
            DownloadRequestStatus.Completed => "Completed",
            DownloadRequestStatus.Failed => "Failed",
            DownloadRequestStatus.InProgress => "In progress",
            DownloadRequestStatus.Cancelled => "Cancelled",
            _ => throw new NotImplementedException("Unsupported request status.")
        }
    );
}
