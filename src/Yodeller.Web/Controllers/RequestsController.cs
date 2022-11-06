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
    public Task<IReadOnlyCollection<GetRequestDto>> Get()
    {
        throw new NotImplementedException(nameof(Get));
    }

    [HttpPost]
    public async Task<IActionResult> Post(NewRequestDto request)
    {
        var command = new RequestDownloadCommand(request.MediaLocator);

        await _mediator.Send(command);

        return NoContent();
    }
}
