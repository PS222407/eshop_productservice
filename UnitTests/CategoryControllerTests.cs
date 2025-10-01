using eshop_productservice.Controllers;
using eshop_productservice.Interfaces;
using eshop_productservice.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests;

public class CategoryControllerTests
{
    private readonly Mock<ICategoryService> _mockCategoryService;
    private readonly CategoryController _controller;

    public CategoryControllerTests()
    {
        _mockCategoryService = new Mock<ICategoryService>();
        _controller = new CategoryController(_mockCategoryService.Object);
    }

    [Fact]
    public async Task Index_ReturnsAllCategories()
    {
        // Arrange
        var categories = new List<CategoryViewModel>
        {
            new() { Id = "1", Name = "Electronics", Count = 3 },
            new() { Id = "2", Name = "Books", Count = 10 }
        };
        _mockCategoryService.Setup(s => s.Get()).ReturnsAsync(categories);

        // Act
        var result = await _controller.Index();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var value = okResult.Value as List<CategoryViewModel>;
        value.Should().BeEquivalentTo(categories);
    }

    [Fact]
    public async Task Index_ReturnsEmptyList()
    {
        // Arrange
        _mockCategoryService.Setup(s => s.Get()).ReturnsAsync(new List<CategoryViewModel>());

        // Act
        var result = await _controller.Index();

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var value = okResult.Value as List<CategoryViewModel>;
        value.Should().BeEmpty();
    }

    [Fact]
    public async Task Index_ById_ReturnsCategory()
    {
        // Arrange
        var category = new CategoryViewModel { Id = "1", Name = "Electronics", Count = 3 };
        _mockCategoryService.Setup(s => s.Get("1")).ReturnsAsync(category);

        // Act
        var result = await _controller.Index("1");

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var value = okResult.Value as CategoryViewModel;
        value.Should().BeEquivalentTo(category);
    }

    [Fact]
    public async Task Index_ById_ReturnsNotFound()
    {
        // Arrange
        _mockCategoryService.Setup(s => s.Get("999")).ReturnsAsync((CategoryViewModel?)null);

        // Act
        var result = await _controller.Index("999");

        // Assert
        var notFoundResult = result.Result as NotFoundResult;
        notFoundResult.Should().NotBeNull();
    }
}

