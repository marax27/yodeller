using Microsoft.AspNetCore.JsonPatch;
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
    public Task<IReadOnlyCollection<GetRequestDto>> Get()
    {
        throw new NotImplementedException(nameof(Get));
    }

    [HttpPost]
    public Task Post(NewRequestDto request)
    {
        throw new NotImplementedException(nameof(Post));
    }

    [HttpPatch("{requestId}")]
    public Task Patch(string requestId, JsonPatchDocument<EditRequestDto> requestPatch)
    {
        throw new NotImplementedException(nameof(Patch));
    }
}
