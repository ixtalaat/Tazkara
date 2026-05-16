using Tazkara.Application.DTOs.Event;
using Tazkara.Application.Wrappers;

namespace Tazkara.Application.Interfaces
{
    public interface IEventService
    {
        Task<ApiResponse<EventDto>> CreateEventAsync(CreateEventRequest request, Guid organizerId);
        Task<ApiResponse<EventDto>> UpdateEventAsync(UpdateEventRequest request, Guid organizerId);
        Task<ApiResponse<bool>> DeleteEventAsync(Guid eventId, Guid organizerId);
        Task<ApiResponse<bool>> PublishEventAsync(Guid eventId, Guid organizerId);
        Task<ApiResponse<bool>> CancelEventAsync(Guid eventId, Guid organizerId);
        Task<ApiResponse<EventDto>> GetEventByIdAsync(Guid eventId);
        Task<ApiResponse<PaginatedResponse<EventDto>>> BrowseEventsAsync(EventFilterRequest filter);
        Task<ApiResponse<List<EventDto>>> GetOrganizerEventsAsync(Guid organizerId);
    }
}
