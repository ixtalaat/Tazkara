using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tazkara.Application.DTOs.Payment;
using Tazkara.Application.Interfaces;

namespace Tazkara.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("session")]
        public async Task<IActionResult> CreateSession(PaymentSessionRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _paymentService.CreatePaymentSessionAsync(request, userId);

            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify(PaymentVerificationRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _paymentService.VerifyPaymentAsync(request, userId);

            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }
    }
}
