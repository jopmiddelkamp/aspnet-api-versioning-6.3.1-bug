using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bug.Controllers;

[ApiController]
[Route("/.well-known")]
[AllowAnonymous]
public class WellKnownController : ControllerBase
{
    private readonly ILogger<WellKnownController> _logger;

    public WellKnownController(ILogger<WellKnownController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [Route("example.toml")]
    public IActionResult GetToml()
    {
        return Content("TOML DATA");
    }
}