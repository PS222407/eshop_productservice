using eshop_productservice.Controllers;
using eshop_productservice.DTOs;
using eshop_productservice.Interfaces;
using eshop_productservice.Models;
using eshop_productservice.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.ControllerTests;

public class CategoryControllerTests
{
    private readonly CategoryController _controller;
    private readonly Mock<ICategoryService> _mockCategoryService;

    public CategoryControllerTests()
    {
        _mockCategoryService = new Mock<ICategoryService>();
        _controller = new CategoryController(_mockCategoryService.Object);
    }

    [Fact]
    public async Task Index_ReturnsAllCategories()
    {
        // Arrange
        var categories = new List<CategoryWithProductCountDto>
        {
            new() { Id = "1", Name = "Electronics", Count = 3 },
            new() { Id = "2", Name = "Books", Count = 10 }
        };
        var expectedCategories = new List<CategoryViewModel>
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
        value.Should().BeEquivalentTo(expectedCategories);
    }

    [Fact]
    public async Task Index_ReturnsEmptyList()
    {
        // Arrange
        _mockCategoryService.Setup(s => s.Get()).ReturnsAsync([]);

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
        var category = new Category { Id = new Guid("68dd3a02-f6f8-832c-a715-2d9902a28601"), Name = "Electronics" };
        var expectedCategory = new CategoryViewModel
            { Id = "68dd3a02-f6f8-832c-a715-2d9902a28601", Name = "Electronics" };
        _mockCategoryService.Setup(s => s.Get("68dd3a02-f6f8-832c-a715-2d9902a28601")).ReturnsAsync(category);

        // Act
        var result = await _controller.Index("68dd3a02-f6f8-832c-a715-2d9902a28601");

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var value = okResult.Value as CategoryViewModel;
        value.Should().BeEquivalentTo(expectedCategory);
    }

    [Fact]
    public async Task Index_ById_ReturnsNotFound()
    {
        // Arrange
        _mockCategoryService.Setup(s => s.Get("999")).ReturnsAsync((Category?)null);

        // Act
        var result = await _controller.Index("999");

        // Assert
        var notFoundResult = result.Result as NotFoundResult;
        notFoundResult.Should().NotBeNull();
    }
}