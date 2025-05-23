using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LinkLeaf.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SecuredController : ControllerBase
{
    [Authorize]
    [HttpGet]
    public ActionResult Get()
    {
        var email = User.FindFirstValue(ClaimTypes.Email);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Ok($"You are logged in as {email} with id {userId}");
    }
}
