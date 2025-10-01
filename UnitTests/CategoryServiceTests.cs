using eshop_productservice.DTOs;
using eshop_productservice.Interfaces;
using eshop_productservice.Models;
using eshop_productservice.Services;
using eshop_productservice.ViewModels;
using FluentAssertions;
using Moq;

namespace UnitTests;

public class CategoryServiceTests
{
    private readonly CategoryService _categoryService;
    private readonly Mock<ICategoryRepository> _mockRepository;

    public CategoryServiceTests()
    {
        _mockRepository = new Mock<ICategoryRepository>();
        _categoryService = new CategoryService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAsync_ReturnsAllCategories()
    {
        // Arrange
        var repositoryCategories = new List<CategoryWithProductCountDto>
        {
            new() { Id = "68dd3a02-f6f8-832c-a715-2d9902a28601", Name = "Electronics", Count = 3 },
            new() { Id = "68dcde78-e9e0-8322-b735-ee59674a4cff", Name = "Books", Count = 23 }
        };
        var expectedCategories = new List<CategoryViewModel>
        {
            new() { Id = "68dd3a02-f6f8-832c-a715-2d9902a28601", Name = "Electronics", Count = 3 },
            new() { Id = "68dcde78-e9e0-8322-b735-ee59674a4cff", Name = "Books", Count = 23 }
        };
        _mockRepository.Setup(r => r.GetAsync()).ReturnsAsync(repositoryCategories);

        // Act
        var result = await _categoryService.Get();

        // Assert
        result.Should().BeEquivalentTo(expectedCategories);
        _mockRepository.Verify(r => r.GetAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WithId_ReturnsCategory()
    {
        // Arrange
        var categoryId = new Guid("68dd3a02-f6f8-832c-a715-2d9902a28601");
        var category = new Category { Id = categoryId, Name = "Electronics" };
        _mockRepository.Setup(r => r.GetAsync(categoryId.ToString())).ReturnsAsync(category);

        // Act
        var result = await _categoryService.Get(categoryId.ToString());

        // Assert
        result.Should().BeEquivalentTo(category);
        _mockRepository.Verify(r => r.GetAsync(categoryId.ToString()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WithId_ReturnsNull_WhenCategoryNotFound()
    {
        // Arrange
        const string categoryId = "999";
        _mockRepository.Setup(r => r.GetAsync(categoryId)).ReturnsAsync((Category?)null);

        // Act
        var result = await _categoryService.Get(categoryId);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.GetAsync(categoryId), Times.Once);
    }
}