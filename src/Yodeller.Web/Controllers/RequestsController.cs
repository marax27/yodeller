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
        var command = new RequestDownloadCommand(mediaLocator);

        await _mediator.Send(command);

        return NoContent();
    }

    [HttpPost]
    public async Task<IActionResult> RequestByPost([FromBody] NewRequestDto request)
    {
        var command = new RequestDownloadCommand(request.MediaLocator);

        await _mediator.Send(command);

        return NoContent();
    }
}
