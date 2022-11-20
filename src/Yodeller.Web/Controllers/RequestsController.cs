using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yodeller.Application.Features;
using Yodeller.Web.Features;

namespace Yodeller.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class RequestsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<RequestsController> _logger;

    public RequestsController(IMediator mediator, ILogger<RequestsController> logger)
    {
        _logger = logger;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IReadOnlyCollection<GetAllRequestsQuery.DownloadRequestDto>> Get()
    {
        var result = await _mediator.Send(new GetAllRequestsQuery());

        return result.Requests;
    }

    [HttpGet("{mediaLocator}")]
    public async Task<IActionResult> RequestByGet([FromRoute] string mediaLocator)
    {
        var command = new RequestDownloadCommand(Array.Empty<string>(), mediaLocator, false);

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> RequestByPost([FromBody] NewRequestDto request)
    {
        var command = new RequestDownloadCommand(
            request.SubtitlePatterns ?? Array.Empty<string>(),
            request.MediaLocator,
            request.AudioOnly);

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpDelete]
    [Route("{requestId}")]
    public async Task<IActionResult> Cancel([FromRoute] string requestId)
    {
        var command = new CancelDownloadCommand(requestId);

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpPost]
    [Route("clear-finished")]
    public async Task<IActionResult> PostClearFinished()
    {
        var command = new ClearFinishedCommand();

        await _mediator.Send(command);

        return NoContent();
    }
}
