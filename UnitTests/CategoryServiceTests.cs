using eshop_productservice.Models;
using eshop_productservice.Projections;
using eshop_productservice.Repositories;
using eshop_productservice.Services;
using eshop_productservice.ViewModels;
using Moq;

namespace UnitTests;

public class CategoryServiceTests
{
    private readonly Mock<CategoryRepository> _mockRepository;
    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        _mockRepository = new Mock<CategoryRepository>();
        _categoryService = new CategoryService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAsync_ReturnsAllCategories()
    {
        // Arrange
        var repositoryCategories = new List<CategoryWithProductCount>
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
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(expectedCategories, result);
        _mockRepository.Verify(r => r.GetAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WithId_ReturnsCategory()
    {
        // Arrange
        var categoryId = new Guid("68dd3a02-f6f8-832c-a715-2d9902a28601");
        var expectedCategory = new CategoryViewModel { Id = categoryId.ToString(), Name = "Electronics" };
        var repositoryCategory = new Category { Id = categoryId, Name = "Electronics" };
        _mockRepository.Setup(r => r.GetAsync(categoryId.ToString())).ReturnsAsync(repositoryCategory);

        // Act
        var result = await _categoryService.Get(categoryId.ToString());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(categoryId.ToString(), result.Id);
        Assert.Equal("Electronics", result.Name);
        Assert.Equal(expectedCategory, result);
        _mockRepository.Verify(r => r.GetAsync(categoryId.ToString()), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WithId_ReturnsNull_WhenCategoryNotFound()
    {
        // Arrange
        var categoryId = "999";
        _mockRepository.Setup(r => r.GetAsync(categoryId)).ReturnsAsync((Category?)null);

        // Act
        var result = await _categoryService.Get(categoryId);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.GetAsync(categoryId), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_CallsRepositoryCreate()
    {
        // Arrange
        var category = new Category { Id = "1", Name = "New Category" };
        _mockRepository.Setup(r => r.CreateAsync(category)).Returns(Task.CompletedTask);

        // Act
        await _categoryService.CreateAsync(category);

        // Assert
        _mockRepository.Verify(r => r.CreateAsync(category), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_CallsRepositoryUpdate()
    {
        // Arrange
        var categoryId = "1";
        var category = new Category { Id = categoryId, Name = "Updated Category" };
        _mockRepository.Setup(r => r.UpdateAsync(categoryId, category)).Returns(Task.CompletedTask);

        // Act
        await _categoryService.UpdateAsync(categoryId, category);

        // Assert
        _mockRepository.Verify(r => r.UpdateAsync(categoryId, category), Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_CallsRepositoryRemove()
    {
        // Arrange
        var categoryId = "1";
        _mockRepository.Setup(r => r.RemoveAsync(categoryId)).Returns(Task.CompletedTask);

        // Act
        await _categoryService.RemoveAsync(categoryId);

        // Assert
        _mockRepository.Verify(r => r.RemoveAsync(categoryId), Times.Once);
    }
}