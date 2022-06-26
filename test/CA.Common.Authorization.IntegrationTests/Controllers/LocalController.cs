using CA.Common.Authorization.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CA.Common.Authorization.IntegrationTests.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class LocalController : ControllerBase
    {
        private readonly IPolicyOperations _policyOperations;

        public LocalController(IPolicyOperations policyOperations)
        {
            _policyOperations = policyOperations;
        }

        [HttpGet("secure")]
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

        [HttpGet("/policy")]
        public async Task<IActionResult> EvaluateUser()
        {
            var result = await _policyOperations.EvaluateAsync(User);
            return Ok(result);
        }
    }
}
