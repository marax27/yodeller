using Core.Shared.StateManagement;
using MediatR;
using Yodeller.Application.Ports;
using Yodeller.Application.State;

namespace Yodeller.Application.Features.CancelRequest;

public record CancelRequestCommand(string RequestId) : IRequest;

public class CancelRequestCommandHandler : IRequestHandler<CancelRequestCommand, Unit>
{
    private readonly IMessageProducer<IStateReducer<DownloadRequestsState>> _messageProducer;

    public CancelRequestCommandHandler(IMessageProducer<IStateReducer<DownloadRequestsState>> messageProducer)
    {
        _messageProducer = messageProducer;
    }

    public Task<Unit> Handle(CancelRequestCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RequestId))
            throw new ArgumentException("Empty Request ID: value is null or whitespace only.");

        var reducer = new CancelRequestReducer(request.RequestId);

        _messageProducer.Produce(reducer);

        return Task.FromResult(Unit.Value);
    }
}
