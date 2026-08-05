using FluentAssertions;
using Moq;
using Tazkara.Application.DTOs.Category;
using Tazkara.Application.Interfaces;
using Tazkara.Application.Mappings;
using Tazkara.Application.Services;
using Tazkara.Domain.Entities;
using Xunit;

namespace Tazkara.Application.Tests;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _repository = new();
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        MappingProfile.RegisterMappings();
        _service = new CategoryService(_repository.Object);
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenNameIsBlank_ReturnsValidationError()
    {
        var result = await _service.CreateCategoryAsync(new CreateCategoryRequest { Name = "  " });

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Category name is required.");
        _repository.Verify(x => x.AddAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenNameAlreadyExists_ReturnsConflictError()
    {
        _repository.Setup(x => x.CategoryExistsAsync("Music")).ReturnsAsync(true);

        var result = await _service.CreateCategoryAsync(new CreateCategoryRequest { Name = "Music" });

        result.Success.Should().BeFalse();
        result.Message.Should().Be("Category already exists.");
        _repository.Verify(x => x.AddAsync(It.IsAny<Category>()), Times.Never);
    }

    [Fact]
    public async Task CreateCategoryAsync_WhenValidRequest_PersistsAndReturnsCategory()
    {
        _repository.Setup(x => x.CategoryExistsAsync("Music")).ReturnsAsync(false);
        _repository.Setup(x => x.AddAsync(It.IsAny<Category>()))
            .ReturnsAsync((Category category) => category);

        var result = await _service.CreateCategoryAsync(new CreateCategoryRequest { Name = "Music" });

        result.Success.Should().BeTrue();
        result.Data!.Name.Should().Be("Music");
        _repository.Verify(x => x.AddAsync(It.Is<Category>(c => c.Name == "Music")), Times.Once);
    }
}
