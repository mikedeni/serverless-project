using Microsoft.AspNetCore.Mvc;

namespace ConstructionSaaS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("crash")]
    public IActionResult Crash()
    {
        throw new Exception("Intentional test error for monitoring demonstration");
    }
}
