using MediatR;
using Yodeller.Application.Messages;
using Yodeller.Application.Ports;

namespace Yodeller.Application.Features;

public record CancelDownloadCommand(string RequestId) : IRequest;

public class CancelDownloadCommandHandler : IRequestHandler<CancelDownloadCommand, Unit>
{
    private readonly IMessageProducer<BaseMessage> _messageProducer;

    public CancelDownloadCommandHandler(IMessageProducer<BaseMessage> messageProducer)
    {
        _messageProducer = messageProducer;
    }

    public Task<Unit> Handle(CancelDownloadCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.RequestId))
            throw new ArgumentException("Empty Request ID: value is null or whitespace only.");

        _messageProducer.Produce(new RequestedDownloadCancellation(request.RequestId));

        return Task.FromResult(Unit.Value);
    }
}
