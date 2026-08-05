using FluentAssertions;
using Moq;
using Tazkara.Application.DTOs.Event;
using Tazkara.Application.Interfaces;
using Tazkara.Application.Mappings;
using Tazkara.Application.Services;
using Tazkara.Domain.Entities;
using Xunit;

namespace Tazkara.Application.Tests;

public class EventServiceTests
{
    private readonly Mock<IEventRepository> _events = new();
    private readonly Mock<ICategoryRepository> _categories = new();
    private readonly EventService _service;

    public EventServiceTests()
    {
        MappingProfile.RegisterMappings();
        _service = new EventService(_events.Object, _categories.Object);
    }

    [Fact]
    public async Task CreateEventAsync_WhenCategoryDoesNotExist_ReturnsError()
    {
        var categoryId = Guid.NewGuid();
        _categories.Setup(x => x.GetByIdAsync(categoryId)).ReturnsAsync((Category?)null);

        var result = await _service.CreateEventAsync(new CreateEventRequest { CategoryId = categoryId }, Guid.NewGuid());

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Category not found.");
        _events.Verify(x => x.AddAsync(It.IsAny<Event>()), Times.Never);
    }

    [Fact]
    public async Task UpdateEventAsync_WhenOrganizerDoesNotOwnEvent_ReturnsAuthorizationError()
    {
        var eventId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        _events.Setup(x => x.GetByIdAsync(eventId)).ReturnsAsync(new Event
        {
            Id = eventId,
            OrganizerId = Guid.NewGuid(),
            CategoryId = categoryId,
            Capacity = 10,
            AvailableTickets = 10
        });

        var result = await _service.UpdateEventAsync(new UpdateEventRequest
        {
            Id = eventId,
            CategoryId = categoryId,
            Capacity = 20
        }, Guid.NewGuid());

        result.Success.Should().BeFalse();
        result.Message.Should().Be("You are not authorized to update this event.");
        _events.Verify(x => x.UpdateAsync(It.IsAny<Event>()), Times.Never);
    }

    [Fact]
    public async Task UpdateEventAsync_WhenCapacityDropsBelowReservedTickets_ReturnsError()
    {
        var eventId = Guid.NewGuid();
        var organizerId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        _events.Setup(x => x.GetByIdAsync(eventId)).ReturnsAsync(new Event
        {
            Id = eventId,
            OrganizerId = organizerId,
            CategoryId = categoryId,
            Capacity = 10,
            AvailableTickets = 2
        });
        _categories.Setup(x => x.GetByIdAsync(categoryId)).ReturnsAsync(new Category { Id = categoryId, Name = "Music" });

        var result = await _service.UpdateEventAsync(new UpdateEventRequest
        {
            Id = eventId,
            CategoryId = categoryId,
            Capacity = 5
        }, organizerId);

        result.Success.Should().BeFalse();
        result.Message.Should().Be("New capacity is lower than already reserved tickets.");
        _events.Verify(x => x.UpdateAsync(It.IsAny<Event>()), Times.Never);
    }
}
