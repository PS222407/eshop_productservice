
using eshop_productservice.Interfaces;
using eshop_productservice.Models;
using eshop_productservice.Services;
using eshop_productservice.Requests;
using eshop_productservice.ViewModels;
using Moq;
using Xunit;

namespace UnitTests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _mockRepository;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockRepository = new Mock<IProductRepository>();
        _productService = new ProductService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetAsync_ReturnsAllProducts()
    {
        // Arrange
        var expectedProducts = new List<Product>
        {
            new Product { Id = "1", Name = "Product 1", Price = 10.99m },
            new Product { Id = "2", Name = "Product 2", Price = 20.99m }
        };
        _mockRepository.Setup(r => r.GetAsync()).ReturnsAsync(expectedProducts);

        // Act
        var result = await _productService.GetAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal(expectedProducts, result);
        _mockRepository.Verify(r => r.GetAsync(), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WithId_ReturnsProduct()
    {
        // Arrange
        var productId = "1";
        var expectedProduct = new Product { Id = productId, Name = "Product 1", Price = 10.99m };
        _mockRepository.Setup(r => r.GetAsync(productId)).ReturnsAsync(expectedProduct);

        // Act
        var result = await _productService.GetAsync(productId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal("Product 1", result.Name);
        _mockRepository.Verify(r => r.GetAsync(productId), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WithId_ReturnsNull_WhenProductNotFound()
    {
        // Arrange
        var productId = "999";
        _mockRepository.Setup(r => r.GetAsync(productId)).ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.GetAsync(productId);

        // Assert
        Assert.Null(result);
        _mockRepository.Verify(r => r.GetAsync(productId), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WithIdList_ReturnsProducts()
    {
        // Arrange
        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var expectedProducts = new List<Product>
        {
            new Product { Id = ids[0].ToString(), Name = "Product 1", Price = 10.99m },
            new Product { Id = ids[1].ToString(), Name = "Product 2", Price = 20.99m }
        };
        _mockRepository.Setup(r => r.GetAsync(ids)).ReturnsAsync(expectedProducts);

        // Act
        var result = await _productService.GetAsync(ids);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        _mockRepository.Verify(r => r.GetAsync(ids), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_CallsRepositoryCreate()
    {
        // Arrange
        var product = new Product { Id = "1", Name = "New Product", Price = 15.99m };
        _mockRepository.Setup(r => r.CreateAsync(product)).Returns(Task.CompletedTask);

        // Act
        await _productService.CreateAsync(product);

        // Assert
        _mockRepository.Verify(r => r.CreateAsync(product), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_CallsRepositoryUpdate()
    {
        // Arrange
        var productId = "1";
        var product = new Product { Id = productId, Name = "Updated Product", Price = 25.99m };
        _mockRepository.Setup(r => r.UpdateAsync(productId, product)).Returns(Task.CompletedTask);

        // Act
        await _productService.UpdateAsync(productId, product);

        // Assert
        _mockRepository.Verify(r => r.UpdateAsync(productId, product), Times.Once);
    }

    [Fact]
    public async Task RemoveAsync_CallsRepositoryRemove()
    {
        // Arrange
        var productId = "1";
        _mockRepository.Setup(r => r.RemoveAsync(productId)).Returns(Task.CompletedTask);

        // Act
        await _productService.RemoveAsync(productId);

        // Assert
        _mockRepository.Verify(r => r.RemoveAsync(productId), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_ReturnsPagedResults()
    {
        // Arrange
        var searchRequest = new SearchRequest { Page = 1, PageSize = 10, SearchTerm = "test" };
        var expectedViewModel = new PaginationViewModel<Product>
        {
            Items = new List<Product>
            {
                new Product { Id = "1", Name = "Test Product", Price = 10.99m }
            },
            TotalCount = 1,
            Page = 1,
            PageSize = 10
        };
        _mockRepository.Setup(r => r.SearchAsync(searchRequest)).ReturnsAsync(expectedViewModel);

        // Act
        var result = await _productService.SearchAsync(searchRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(1, result.TotalCount);
        _mockRepository.Verify(r => r.SearchAsync(searchRequest), Times.Once);
    }
}
