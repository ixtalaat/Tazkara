using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tazkara.Application.DTOs.Ticket;
using Tazkara.Application.Interfaces;

namespace Tazkara.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketsController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [HttpPost("reserve")]
        public async Task<IActionResult> Reserve(ReserveTicketRequest request)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _ticketService.ReserveTicketAsync(request, userId);

            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _ticketService.CancelReservationAsync(id, userId);

            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpGet("my-tickets")]
        public async Task<IActionResult> GetMyTickets()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _ticketService.GetUserTicketsAsync(userId);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _ticketService.GetTicketByIdAsync(id);
            if (response.Success)
                return Ok(response);

            return NotFound(response);
        }
    }
}
