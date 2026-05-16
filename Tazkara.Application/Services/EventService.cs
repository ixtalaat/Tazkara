using Mapster;
using Tazkara.Application.DTOs.Event;
using Tazkara.Application.Interfaces;
using Tazkara.Application.Wrappers;
using Tazkara.Domain.Entities;
using Tazkara.Domain.Enums;

namespace Tazkara.Application.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly ICategoryRepository _categoryRepository;

        public EventService(IEventRepository eventRepository, ICategoryRepository categoryRepository)
        {
            _eventRepository = eventRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<ApiResponse<EventDto>> CreateEventAsync(CreateEventRequest request, Guid organizerId)
        {
            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
                return ApiResponse<EventDto>.ErrorResponse("Category not found.");

            var newEvent = new Event
            {
                Title = request.Title,
                Description = request.Description,
                Location = request.Location,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Capacity = request.Capacity,
                AvailableTickets = request.Capacity,
                Price = request.Price,
                CategoryId = request.CategoryId,
                OrganizerId = organizerId,
                Status = EventStatus.Draft
            };

            await _eventRepository.AddAsync(newEvent);

            return await GetEventByIdAsync(newEvent.Id);
        }

        public async Task<ApiResponse<EventDto>> UpdateEventAsync(UpdateEventRequest request, Guid organizerId)
        {
            var existingEvent = await _eventRepository.GetByIdAsync(request.Id);
            if (existingEvent == null)
                return ApiResponse<EventDto>.ErrorResponse("Event not found.");

            if (existingEvent.OrganizerId != organizerId)
                return ApiResponse<EventDto>.ErrorResponse("You are not authorized to update this event.");

            var category = await _categoryRepository.GetByIdAsync(request.CategoryId);
            if (category == null)
                return ApiResponse<EventDto>.ErrorResponse("Category not found.");

            existingEvent.Title = request.Title;
            existingEvent.Description = request.Description;
            existingEvent.Location = request.Location;
            existingEvent.StartDate = request.StartDate;
            existingEvent.EndDate = request.EndDate;
            
            int capacityDiff = request.Capacity - existingEvent.Capacity;
            existingEvent.Capacity = request.Capacity;
            existingEvent.AvailableTickets += capacityDiff;

            if (existingEvent.AvailableTickets < 0)
                return ApiResponse<EventDto>.ErrorResponse("New capacity is lower than already reserved tickets.");

            existingEvent.Price = request.Price;
            existingEvent.CategoryId = request.CategoryId;

            await _eventRepository.UpdateAsync(existingEvent);
            return await GetEventByIdAsync(existingEvent.Id);
        }

        public async Task<ApiResponse<bool>> DeleteEventAsync(Guid eventId, Guid organizerId)
        {
            var existingEvent = await _eventRepository.GetByIdAsync(eventId);
            if (existingEvent == null)
                return ApiResponse<bool>.ErrorResponse("Event not found.");

            if (existingEvent.OrganizerId != organizerId)
                return ApiResponse<bool>.ErrorResponse("You are not authorized to delete this event.");

            await _eventRepository.DeleteAsync(existingEvent);
            return ApiResponse<bool>.SuccessResponse(true);
        }

        public async Task<ApiResponse<bool>> PublishEventAsync(Guid eventId, Guid organizerId)
        {
            var existingEvent = await _eventRepository.GetByIdAsync(eventId);
            if (existingEvent == null) return ApiResponse<bool>.ErrorResponse("Event not found.");
            if (existingEvent.OrganizerId != organizerId) return ApiResponse<bool>.ErrorResponse("Unauthorized.");

            existingEvent.Status = EventStatus.Published;
            await _eventRepository.UpdateAsync(existingEvent);
            return ApiResponse<bool>.SuccessResponse(true);
        }

        public async Task<ApiResponse<bool>> CancelEventAsync(Guid eventId, Guid organizerId)
        {
            var existingEvent = await _eventRepository.GetByIdAsync(eventId);
            if (existingEvent == null) return ApiResponse<bool>.ErrorResponse("Event not found.");
            if (existingEvent.OrganizerId != organizerId) return ApiResponse<bool>.ErrorResponse("Unauthorized.");

            existingEvent.Status = EventStatus.Cancelled;
            await _eventRepository.UpdateAsync(existingEvent);
            return ApiResponse<bool>.SuccessResponse(true);
        }

        public async Task<ApiResponse<EventDto>> GetEventByIdAsync(Guid eventId)
        {
            var existingEvent = await _eventRepository.GetEventWithDetailsAsync(eventId);

            if (existingEvent == null) return ApiResponse<EventDto>.ErrorResponse("Event not found.");

            return ApiResponse<EventDto>.SuccessResponse(existingEvent.Adapt<EventDto>());
        }

        public async Task<ApiResponse<PaginatedResponse<EventDto>>> BrowseEventsAsync(EventFilterRequest filter)
        {
            var (items, totalCount) = await _eventRepository.BrowseEventsAsync(filter);

            var response = new PaginatedResponse<EventDto>
            {
                Items = items.Adapt<List<EventDto>>(),
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

            return ApiResponse<PaginatedResponse<EventDto>>.SuccessResponse(response);
        }

        public async Task<ApiResponse<List<EventDto>>> GetOrganizerEventsAsync(Guid organizerId)
        {
            var events = await _eventRepository.GetOrganizerEventsAsync(organizerId);

            return ApiResponse<List<EventDto>>.SuccessResponse(events.Adapt<List<EventDto>>());
        }
    }
}
