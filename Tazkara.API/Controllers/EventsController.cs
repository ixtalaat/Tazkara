using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tazkara.Application.DTOs.Event;
using Tazkara.Application.Interfaces;

namespace Tazkara.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var response = await _eventService.GetEventByIdAsync(id);
            if (response.Success)
                return Ok(response);
            return NotFound(response);
        }

        [HttpGet]
        public async Task<IActionResult> Browse([FromQuery] EventFilterRequest filter)
        {
            var response = await _eventService.BrowseEventsAsync(filter);
            return Ok(response);
        }

        [Authorize(Roles = "Organizer")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateEventRequest request)
        {
            var organizerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _eventService.CreateEventAsync(request, organizerId);
            
            if (response.Success)
                return CreatedAtAction(nameof(GetById), new { id = response.Data!.Id }, response);
                
            return BadRequest(response);
        }

        [Authorize(Roles = "Organizer")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateEventRequest request)
        {
            if (id != request.Id)
                return BadRequest("ID mismatch");

            var organizerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _eventService.UpdateEventAsync(request, organizerId);

            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }

        [Authorize(Roles = "Organizer")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var organizerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _eventService.DeleteEventAsync(id, organizerId);

            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }

        [Authorize(Roles = "Organizer")]
        [HttpPatch("{id}/publish")]
        public async Task<IActionResult> Publish(Guid id)
        {
            var organizerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _eventService.PublishEventAsync(id, organizerId);

            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }

        [Authorize(Roles = "Organizer")]
        [HttpPatch("{id}/cancel")]
        public async Task<IActionResult> Cancel(Guid id)
        {
            var organizerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _eventService.CancelEventAsync(id, organizerId);

            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }

        [Authorize(Roles = "Organizer")]
        [HttpGet("my-events")]
        public async Task<IActionResult> GetMyEvents()
        {
            var organizerId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var response = await _eventService.GetOrganizerEventsAsync(organizerId);
            return Ok(response);
        }
    }
}
