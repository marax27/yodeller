using MediatR;
using Yodeller.Application.Models;
using Yodeller.Application.Ports;

namespace Yodeller.Application.Features;

public record RequestDownloadCommand(
    string MediaLocator
) : IRequest;


public class RequestDownloadCommandHandler : IRequestHandler<RequestDownloadCommand>
{
    private readonly IRequestRepository _requestRepository;
    private readonly IClock _clock;

    public RequestDownloadCommandHandler(IRequestRepository requestRepository, IClock clock)
    {
        _requestRepository = requestRepository ?? throw new ArgumentNullException(nameof(requestRepository));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
    }

    public Task<Unit> Handle(RequestDownloadCommand command, CancellationToken cancellationToken)
    {
        var model = MapToModel(command);

        _requestRepository.Add(model);

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
