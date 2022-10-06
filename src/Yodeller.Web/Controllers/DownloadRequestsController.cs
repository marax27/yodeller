using Microsoft.AspNetCore.Mvc;
using Yodeller.Web.Features;

namespace Yodeller.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class DownloadRequestsController : ControllerBase
{
    private readonly ILogger<DownloadRequestsController> _logger;

    public DownloadRequestsController(ILogger<DownloadRequestsController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetAllDownloadRequests")]
    public IEnumerable<GetDownloadRequestDto> Get()
    {
        return Array.Empty<GetDownloadRequestDto>();
    }
}
