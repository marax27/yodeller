using Core.Shared.StateManagement;
using MediatR;
using Yodeller.Application.Ports;
using Yodeller.Application.State;

namespace Yodeller.Application.Features.GetAllRequests;

public record GetAllRequestsQuery : IRequest<GetAllRequestsQuery.Result>
{
    public record HistoryEntryDto(
        string Description,
        DateTime DateTime
    );

    public record DownloadRequestDto(
        string Id,
        string MediaLocator,
        bool AudioOnly,
        IReadOnlyCollection<HistoryEntryDto> History,
        string Status
    );

    public record Result(
        IReadOnlyCollection<DownloadRequestDto> Requests
    );
}

public class GetAllRequestsQueryHandler : IRequestHandler<GetAllRequestsQuery, GetAllRequestsQuery.Result>
{
    private readonly IMessageProducer<IStateReducer<DownloadRequestsState>> _messageProducer;

    public GetAllRequestsQueryHandler(IMessageProducer<IStateReducer<DownloadRequestsState>> messageProducer)
    {
        _messageProducer = messageProducer ?? throw new ArgumentNullException(nameof(messageProducer));
    }

    public async Task<GetAllRequestsQuery.Result> Handle(GetAllRequestsQuery request, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<GetAllRequestsQuery.Result>();
        var reducer = new GetAllRequestsReducer(tcs.SetResult);

        _messageProducer.Produce(reducer);

        var result = await tcs.Task;
        return result;
    }
}
