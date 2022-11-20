using MediatR;
using Yodeller.Application.Messages;
using Yodeller.Application.Ports;

namespace Yodeller.Application.Features;

public record ClearFinishedCommand : IRequest;

public class ClearFinishedCommandHandler : IRequestHandler<ClearFinishedCommand, Unit>
{
    private readonly IMessageProducer<BaseMessage> _messageProducer;

    public ClearFinishedCommandHandler(IMessageProducer<BaseMessage> messageProducer)
    {
        _messageProducer = messageProducer ?? throw new ArgumentNullException(nameof(messageProducer));
    }

    public Task<Unit> Handle(ClearFinishedCommand request, CancellationToken cancellationToken)
    {
        _messageProducer.Produce(new ClearFinishedRequests());

        return Task.FromResult(Unit.Value);
    }
}
