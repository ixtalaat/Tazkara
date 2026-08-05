namespace Tazkara.Application.DTOs.Admin;

public sealed class AdminOverviewDto
{
    public int TotalUsers { get; set; }
    public int TotalOrganizers { get; set; }
    public int TotalCustomers { get; set; }
    public int TotalEvents { get; set; }
    public int DraftEvents { get; set; }
    public int PublishedEvents { get; set; }
    public int TotalTicketsSold { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<AdminUserDto> Users { get; set; } = new();
    public List<AdminEventDto> Events { get; set; } = new();
    public List<AdminCategoryDto> Categories { get; set; } = new();
}

public sealed class AdminUserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public sealed class AdminEventDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string OrganizerName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public decimal Price { get; set; }
    public int Capacity { get; set; }
    public int AvailableTickets { get; set; }
    public string Status { get; set; } = string.Empty;
}

public sealed class AdminCategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int EventCount { get; set; }
}
