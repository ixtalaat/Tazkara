using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tazkara.Application.DTOs.Admin;
using Tazkara.Application.DTOs.Category;
using Tazkara.Application.Interfaces;
using Tazkara.Application.Wrappers;
using Tazkara.Domain.Entities;
using Tazkara.Domain.Enums;
using Tazkara.Infrastructure.Data;

namespace Tazkara.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public sealed class AdminController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICategoryService _categoryService;

    public AdminController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, ICategoryService categoryService)
    {
        _db = db;
        _userManager = userManager;
        _categoryService = categoryService;
    }

    [HttpGet("overview")]
    public async Task<IActionResult> Overview()
    {
        var users = await _db.Users.AsNoTracking().OrderByDescending(u => u.CreatedAt).ToListAsync();
        var events = await _db.Events.AsNoTracking().Include(e => e.Category).Include(e => e.Organizer)
            .OrderByDescending(e => e.CreatedAt).ToListAsync();
        var tickets = await _db.Tickets.AsNoTracking().Include(t => t.Payments).ToListAsync();
        var rolesByUser = new Dictionary<Guid, string>();
        foreach (var user in users)
            rolesByUser[user.Id] = (await _userManager.GetRolesAsync(user)).FirstOrDefault() ?? "User";

        var overview = new AdminOverviewDto
        {
            TotalUsers = users.Count,
            TotalOrganizers = rolesByUser.Values.Count(role => role == "Organizer"),
            TotalCustomers = rolesByUser.Values.Count(role => role == "Customer"),
            TotalEvents = events.Count,
            DraftEvents = events.Count(e => e.Status == EventStatus.Draft),
            PublishedEvents = events.Count(e => e.Status == EventStatus.Published),
            TotalTicketsSold = tickets.Count(t => t.Status == TicketStatus.Confirmed || t.Status == TicketStatus.Used),
            TotalRevenue = tickets.SelectMany(t => t.Payments).Where(p => p.Status == PaymentStatus.Paid).Sum(p => p.Amount),
            Users = users.Select(user => new AdminUserDto
            {
                Id = user.Id, Name = $"{user.FirstName} {user.LastName}".Trim(), Email = user.Email ?? "",
                Role = rolesByUser[user.Id], CreatedAt = user.CreatedAt
            }).ToList(),
            Events = events.Select(eventItem => new AdminEventDto
            {
                Id = eventItem.Id, Title = eventItem.Title, OrganizerName = eventItem.Organizer is null ? "" : $"{eventItem.Organizer.FirstName} {eventItem.Organizer.LastName}",
                CategoryName = eventItem.Category?.Name ?? "", StartDate = eventItem.StartDate, Price = eventItem.Price,
                Capacity = eventItem.Capacity, AvailableTickets = eventItem.AvailableTickets, Status = eventItem.Status.ToString()
            }).ToList(),
            Categories = await _db.Categories.AsNoTracking().OrderBy(c => c.Name)
                .Select(category => new AdminCategoryDto { Id = category.Id, Name = category.Name, EventCount = category.Events.Count }).ToListAsync()
        };

        return Ok(ApiResponse<AdminOverviewDto>.SuccessResponse(overview));
    }

    [HttpPatch("events/{id}/publish")]
    public async Task<IActionResult> PublishEvent(Guid id) => await SetEventStatus(id, EventStatus.Published);

    [HttpPatch("events/{id}/reject")]
    public async Task<IActionResult> RejectEvent(Guid id) => await SetEventStatus(id, EventStatus.Cancelled);

    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory(CreateCategoryRequest request)
    {
        var result = await _categoryService.CreateCategoryAsync(request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("categories/{id}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var category = await _db.Categories.FindAsync(id);
        if (category is null) return NotFound(ApiResponse<bool>.ErrorResponse("Category not found."));
        if (await _db.Events.AnyAsync(e => e.CategoryId == id)) return BadRequest(ApiResponse<bool>.ErrorResponse("This category is used by an event."));
        _db.Categories.Remove(category);
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<bool>.SuccessResponse(true, "Category deleted."));
    }

    private async Task<IActionResult> SetEventStatus(Guid id, EventStatus status)
    {
        var eventItem = await _db.Events.FindAsync(id);
        if (eventItem is null) return NotFound(ApiResponse<bool>.ErrorResponse("Event not found."));
        eventItem.Status = status;
        await _db.SaveChangesAsync();
        return Ok(ApiResponse<bool>.SuccessResponse(true));
    }
}
