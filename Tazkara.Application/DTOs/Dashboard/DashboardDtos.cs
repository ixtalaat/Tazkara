using Tazkara.Domain.Enums;

namespace Tazkara.Application.DTOs.Dashboard
{
    public class OrganizerDashboardDto
    {
        public int TotalEvents { get; set; }
        public int TotalTicketsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<OrganizerEventStatsDto> EventStats { get; set; } = new();
    }

    public class OrganizerEventStatsDto
    {
        public Guid EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public EventStatus Status { get; set; }
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        public int TicketsSold { get; set; }
        public int TicketsReserved { get; set; }
        public int TicketsAvailable { get; set; }
        public decimal Revenue { get; set; }
    }
}
