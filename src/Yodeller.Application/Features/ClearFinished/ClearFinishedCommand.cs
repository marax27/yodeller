using Core.Shared.StateManagement;
using MediatR;
using Yodeller.Application.Ports;
using Yodeller.Application.State;

namespace Yodeller.Application.Features.ClearFinished;

public record ClearFinishedCommand : IRequest;

public class ClearFinishedCommandHandler : IRequestHandler<ClearFinishedCommand, Unit>
{
    private readonly IMessageProducer<IStateReducer<DownloadRequestsState>> _messageProducer;

    public ClearFinishedCommandHandler(IMessageProducer<IStateReducer<DownloadRequestsState>> messageProducer)
    {
        _messageProducer = messageProducer ?? throw new ArgumentNullException(nameof(messageProducer));
    }

    public Task<Unit> Handle(ClearFinishedCommand request, CancellationToken cancellationToken)
    {
        var reducer = new ClearFinishedRequestsReducer();

        _messageProducer.Produce(reducer);

        return Task.FromResult(Unit.Value);
    }
}
