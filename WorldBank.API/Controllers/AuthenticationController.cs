using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorldBank.API.Business;
using WorldBank.Shared.RequestModel;

namespace WorldBank.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly IAuthenticationBL authenticationBL;

        public AuthenticationController(IAuthenticationBL authenticationBL)
        {
            this.authenticationBL = authenticationBL;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(PostLoginRequest request)
        {
            var response = await authenticationBL.PostLogin(request);

            if (response.Error == null)
                return Ok(response);
            else
                return StatusCode(StatusCodes.Status401Unauthorized, response);
        }
    }
}

