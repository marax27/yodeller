using MediatR;
using Microsoft.AspNetCore.Mvc;
using Yodeller.Application.Features;
using Yodeller.Web.Features;

namespace Yodeller.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class EnvironmentController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<EnvironmentController> _logger;

    public EnvironmentController(IMediator mediator, ILogger<EnvironmentController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<EnvironmentChecksQuery.Result> Get()
    {
        var result = await _mediator.Send(new EnvironmentChecksQuery());

        return result;
    }
}
