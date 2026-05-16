using Microsoft.AspNetCore.Mvc;
using Tazkara.Application.DTOs.Auth;
using Tazkara.Application.Interfaces;

namespace Tazkara.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var response = await _identityService.RegisterAsync(request);
            if (response.Success)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var response = await _identityService.LoginAsync(request);
            if (response.Success)
            {
                return Ok(response);
            }

            return Unauthorized(response);
        }
    }
}
