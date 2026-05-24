using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tazkara.Application.DTOs.Dashboard;
using Tazkara.Application.Interfaces;
using Tazkara.Application.Wrappers;
using Tazkara.Domain.Enums;

namespace Tazkara.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IEventRepository _eventRepository;
        private readonly ITicketRepository _ticketRepository;

        public DashboardService(IEventRepository eventRepository, ITicketRepository ticketRepository)
        {
            _eventRepository = eventRepository;
            _ticketRepository = ticketRepository;
        }

        public async Task<ApiResponse<OrganizerDashboardDto>> GetOrganizerDashboardAsync(Guid organizerId)
        {
            var events = await _eventRepository.GetOrganizerEventsAsync(organizerId);
            var tickets = await _ticketRepository.GetOrganizerTicketsAsync(organizerId);

            var eventStats = new List<OrganizerEventStatsDto>();
            foreach (var ev in events)
            {
                var eventTickets = tickets.Where(t => t.EventId == ev.Id).ToList();
                
                var soldCount = eventTickets.Count(t => t.Status == TicketStatus.Confirmed || t.Status == TicketStatus.Used);
                var reservedCount = eventTickets.Count(t => t.Status == TicketStatus.Reserved);
                
                var revenue = eventTickets
                    .SelectMany(t => t.Payments)
                    .Where(p => p.Status == PaymentStatus.Paid)
                    .Sum(p => p.Amount);

                eventStats.Add(new OrganizerEventStatsDto
                {
                    EventId = ev.Id,
                    Title = ev.Title,
                    StartDate = ev.StartDate,
                    Status = ev.Status,
                    Price = ev.Price,
                    Capacity = ev.Capacity,
                    TicketsSold = soldCount,
                    TicketsReserved = reservedCount,
                    TicketsAvailable = ev.AvailableTickets,
                    Revenue = revenue
                });
            }

            var totalTicketsSold = eventStats.Sum(e => e.TicketsSold);
            var totalRevenue = eventStats.Sum(e => e.Revenue);

            var dashboardDto = new OrganizerDashboardDto
            {
                TotalEvents = events.Count,
                TotalTicketsSold = totalTicketsSold,
                TotalRevenue = totalRevenue,
                EventStats = eventStats
            };

            return ApiResponse<OrganizerDashboardDto>.SuccessResponse(dashboardDto);
        }
    }
}
