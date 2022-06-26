using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CA.Common.Authorization.IntegrationTests.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class RemoteController : ControllerBase
    {
        [HttpGet("seure")]
        [Authorize(AppPermissions.ViewWeather)]
        public IActionResult GetSecure()
        {
            return Ok("Secure Content");
        }

        [HttpGet("forbidden")]
        [Authorize("TestPermission")]
        public IActionResult Forbiden()
        {
            return Ok("Secure Content");
        }
    }
}
