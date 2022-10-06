using Microsoft.AspNetCore.Mvc;
using Yodeller.Web.Features;

namespace Yodeller.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class RequestsController : ControllerBase
{
    private readonly ILogger<RequestsController> _logger;

    public RequestsController(ILogger<RequestsController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IReadOnlyCollection<GetRequestDto> Get()
    {
        return Array.Empty<GetRequestDto>();
    }
}
