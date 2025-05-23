using Microsoft.AspNetCore.Mvc;

namespace LinkLeaf.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthcheckController : ControllerBase
{
    [HttpGet]
    public ActionResult Healthcheck() =>
        Ok("OK");
}
