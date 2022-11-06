using MediatR;
using Yodeller.Application.Messages;
using Yodeller.Application.Models;
using Yodeller.Application.Ports;

namespace Yodeller.Application.Features;

public record RequestDownloadCommand(
    string MediaLocator
) : IRequest;


public class RequestDownloadCommandHandler : IRequestHandler<RequestDownloadCommand>
{
    private readonly IMessageProducer<BaseMessage> _messageProducer;
    private readonly IClock _clock;

    public RequestDownloadCommandHandler(IMessageProducer<BaseMessage> messageProducer, IClock clock)
    {
        _messageProducer = messageProducer ?? throw new ArgumentNullException(nameof(messageProducer));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public Task<Unit> Handle(RequestDownloadCommand command, CancellationToken cancellationToken)
    {
        var model = MapToModel(command);

        _messageProducer.Produce(new RequestedNewDownload(model));

        return Task.FromResult(Unit.Value);
    }

    private DownloadRequest MapToModel(RequestDownloadCommand command) => new(
        CreateRequestId(),
        _clock.GetNow(),
        command.MediaLocator,
        DownloadRequestStatus.New
    );

    private static string CreateRequestId() => Guid.NewGuid().ToString();
}
