using Core.Shared.StateManagement;
using MediatR;
using Yodeller.Application.Models;
using Yodeller.Application.Ports;
using Yodeller.Application.State;

namespace Yodeller.Application.Features.RequestDownload;

public record RequestDownloadCommand(
    IReadOnlyCollection<string> SubtitlePatterns,
    string MediaLocator,
    bool AudioOnly
) : IRequest;

public class RequestDownloadCommandHandler : IRequestHandler<RequestDownloadCommand>
{
    private readonly IMessageProducer<IStateReducer<DownloadRequestsState>> _messageProducer;
    private readonly IClock _clock;

    public RequestDownloadCommandHandler(IMessageProducer<IStateReducer<DownloadRequestsState>> messageProducer, IClock clock)
    {
        _messageProducer = messageProducer ?? throw new ArgumentNullException(nameof(messageProducer));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public Task<Unit> Handle(RequestDownloadCommand request, CancellationToken cancellationToken)
    {
        var newRequest = MapToModel(request);

        _messageProducer.Produce(new AddNewRequestReducer(newRequest));

        return Task.FromResult(Unit.Value);
    }

    private DownloadRequest MapToModel(RequestDownloadCommand command)
    {
        var timeNow = _clock.GetNow();
        var historyEntry = new HistoryEntry("Requested.", timeNow);
        return new(
            CreateRequestId(),
            timeNow,
            command.MediaLocator,
            command.AudioOnly,
            command.SubtitlePatterns,
            new[] { historyEntry },
            DownloadRequestStatus.New
        );
    }

    private static string CreateRequestId() => Guid.NewGuid().ToString();
}
