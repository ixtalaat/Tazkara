using FluentAssertions;
using Moq;
using System;
using System.Threading.Tasks;
using Tazkara.Application.DTOs.Ticket;
using Tazkara.Application.Exceptions;
using Tazkara.Application.Interfaces;
using Tazkara.Application.Mappings;
using Tazkara.Application.Services;
using Tazkara.Domain.Entities;
using Tazkara.Domain.Enums;
using Xunit;

namespace Tazkara.Application.Tests
{
    public class TicketServiceTests
    {
        private readonly Mock<ITicketRepository> _ticketRepositoryMock;
        private readonly Mock<IEventRepository> _eventRepositoryMock;
        private readonly TicketService _ticketService;

        public TicketServiceTests()
        {
            // Initialize mappings for Mapster Adapt calls
            MappingProfile.RegisterMappings();

            _ticketRepositoryMock = new Mock<ITicketRepository>();
            _eventRepositoryMock = new Mock<IEventRepository>();
            _ticketService = new TicketService(_ticketRepositoryMock.Object, _eventRepositoryMock.Object);
        }

        [Fact]
        public async Task ReserveTicketAsync_WhenValidRequest_ShouldReserveSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var targetEvent = new Event
            {
                Id = eventId,
                Title = "Test Concert",
                Status = EventStatus.Published,
                StartDate = DateTime.UtcNow.AddDays(1),
                AvailableTickets = 10,
                Capacity = 10
            };

            _eventRepositoryMock.Setup(repo => repo.GetByIdAsync(eventId))
                .ReturnsAsync(targetEvent);

            _ticketRepositoryMock.Setup(repo => repo.HasUserBookedEventAsync(userId, eventId))
                .ReturnsAsync(false);

            _eventRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Event>()))
                .Returns(Task.CompletedTask);

            _ticketRepositoryMock.Setup(repo => repo.AddAsync(It.IsAny<Ticket>()))
                .ReturnsAsync((Ticket t) => t);

            _ticketRepositoryMock.Setup(repo => repo.GetTicketWithDetailsAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => new Ticket
                {
                    Id = id,
                    EventId = eventId,
                    UserId = userId,
                    Status = TicketStatus.Reserved,
                    PaymentStatus = PaymentStatus.Pending,
                    TicketNumber = "TZK-TEST-123",
                    Event = targetEvent
                });

            var request = new ReserveTicketRequest { EventId = eventId };

            // Act
            var result = await _ticketService.ReserveTicketAsync(request, userId);

            // Assert
            result.Success.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Status.Should().Be(TicketStatus.Reserved.ToString());
            result.Data.EventTitle.Should().Be("Test Concert");
            targetEvent.AvailableTickets.Should().Be(9);
            _eventRepositoryMock.Verify(repo => repo.UpdateAsync(targetEvent), Times.Once);
            _ticketRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Ticket>()), Times.Once);
        }

        [Fact]
        public async Task ReserveTicketAsync_WhenEventSoldOut_ShouldReturnError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var targetEvent = new Event
            {
                Id = eventId,
                Status = EventStatus.Published,
                StartDate = DateTime.UtcNow.AddDays(1),
                AvailableTickets = 0,
                Capacity = 10
            };

            _eventRepositoryMock.Setup(repo => repo.GetByIdAsync(eventId))
                .ReturnsAsync(targetEvent);

            var request = new ReserveTicketRequest { EventId = eventId };

            // Act
            var result = await _ticketService.ReserveTicketAsync(request, userId);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("This event is sold out.");
            _eventRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Event>()), Times.Never);
            _ticketRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Ticket>()), Times.Never);
        }

        [Fact]
        public async Task ReserveTicketAsync_WhenConcurrencyExceptionOccurs_ShouldReturnConcurrencyError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var targetEvent = new Event
            {
                Id = eventId,
                Status = EventStatus.Published,
                StartDate = DateTime.UtcNow.AddDays(1),
                AvailableTickets = 5,
                Capacity = 10
            };

            _eventRepositoryMock.Setup(repo => repo.GetByIdAsync(eventId))
                .ReturnsAsync(targetEvent);

            _ticketRepositoryMock.Setup(repo => repo.HasUserBookedEventAsync(userId, eventId))
                .ReturnsAsync(false);

            _eventRepositoryMock.Setup(repo => repo.UpdateAsync(It.IsAny<Event>()))
                .ThrowsAsync(new ConcurrencyException("A concurrency error occurred."));

            var request = new ReserveTicketRequest { EventId = eventId };

            // Act
            var result = await _ticketService.ReserveTicketAsync(request, userId);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Contain("Concurrency conflict");
            _ticketRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Ticket>()), Times.Never);
        }

        [Fact]
        public async Task ReserveTicketAsync_WhenEventNotPublished_ShouldReturnError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var targetEvent = new Event
            {
                Id = eventId,
                Status = EventStatus.Draft,
                StartDate = DateTime.UtcNow.AddDays(1),
                AvailableTickets = 10,
                Capacity = 10
            };

            _eventRepositoryMock.Setup(repo => repo.GetByIdAsync(eventId))
                .ReturnsAsync(targetEvent);

            var request = new ReserveTicketRequest { EventId = eventId };

            // Act
            var result = await _ticketService.ReserveTicketAsync(request, userId);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Event is not open for reservations.");
        }

        [Fact]
        public async Task ReserveTicketAsync_WhenUserAlreadyBooked_ShouldReturnError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var eventId = Guid.NewGuid();
            var targetEvent = new Event
            {
                Id = eventId,
                Status = EventStatus.Published,
                StartDate = DateTime.UtcNow.AddDays(1),
                AvailableTickets = 10,
                Capacity = 10
            };

            _eventRepositoryMock.Setup(repo => repo.GetByIdAsync(eventId))
                .ReturnsAsync(targetEvent);

            _ticketRepositoryMock.Setup(repo => repo.HasUserBookedEventAsync(userId, eventId))
                .ReturnsAsync(true);

            var request = new ReserveTicketRequest { EventId = eventId };

            // Act
            var result = await _ticketService.ReserveTicketAsync(request, userId);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("You have already booked a ticket for this event.");
        }
    }
}
