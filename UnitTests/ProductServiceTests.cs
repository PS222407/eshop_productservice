using eshop_productservice.Interfaces;
using eshop_productservice.Models;
using eshop_productservice.Requests;
using eshop_productservice.Services;
using eshop_productservice.ViewModels;
using FluentAssertions;
using Moq;

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
    public async Task GetAsync_WithId_ReturnsProduct()
    {
        // Arrange
        const string productId = "68dd3a02-f6f8-832c-a715-2d9902a28601";
        var expectedProduct = new Product
        { 
            Id = productId, 
            Name = "Laptop", 
            PriceInCents = 99999,
            StarsTimesTen = 46,
            ImageUrl = "https://example.com/laptop.jpg"
        };
        _mockRepository.Setup(r => r.GetAsync(productId)).ReturnsAsync(expectedProduct);

        // Act
        var result = await _productService.GetAsync(productId);

        // Assert
        result.Should().BeEquivalentTo(expectedProduct);
        _mockRepository.Verify(r => r.GetAsync(productId), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WithId_ReturnsNull_WhenProductNotFound()
    {
        // Arrange
        const string productId = "999";
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
        var ids = new List<Guid> 
        { 
            Guid.Parse("68dd3a02-f6f8-832c-a715-2d9902a28601"), 
            Guid.Parse("68dcde78-e9e0-8322-b735-ee59674a4cff") 
        };
        var expectedProducts = new List<Product>
        {
            new() { Id = ids[0].ToString(), Name = "Laptop", PriceInCents = 99999, StarsTimesTen = 46, ImageUrl = "https://example.com/laptop.jpg" },
            new() { Id = ids[1].ToString(), Name = "Mouse", PriceInCents = 1999, StarsTimesTen = 40, ImageUrl = "https://example.com/mouse.jpg" }
        };
        _mockRepository.Setup(r => r.GetAsync(ids)).ReturnsAsync(expectedProducts);

        // Act
        var result = await _productService.GetAsync(ids);

        // Assert
        result.Should().BeEquivalentTo(expectedProducts);
        _mockRepository.Verify(r => r.GetAsync(ids), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WithIdList_ReturnsEmptyList_WhenNoProductsFound()
    {
        // Arrange
        var ids = new List<Guid> 
        { 
            Guid.Parse("68dd3a02-f6f8-832c-a715-2d9902a28601")
        };
        var emptyList = new List<Product>();
        _mockRepository.Setup(r => r.GetAsync(ids)).ReturnsAsync(emptyList);

        // Act
        var result = await _productService.GetAsync(ids);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockRepository.Verify(r => r.GetAsync(ids), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_ReturnsPagedProducts()
    {
        // Arrange
        var searchRequest = new SearchRequest 
        { 
            q = "brush",
            filter_by = "{\"categories\": [\"019961f6-16b2-7aaf-8543-457fe1dbf084\"]}",
            sort_by = null,
            page = 1, 
            per_page = 12, 
        };
        var expectedViewModel = new PaginationViewModel<Product>
        {
            found = 2,
            page = 1,
            hits = new List<Product>
            {
                new() { Id = "1", Name = "Gaming Laptop", PriceInCents = 129999, StarsTimesTen = 48, ImageUrl = "https://example.com/laptop.jpg" },
                new() { Id = "2", Name = "Business Laptop", PriceInCents = 89999, StarsTimesTen = 46, ImageUrl = "https://example.com/laptop.jpg" },
            },
            request_params = new RequestParams
            {
                per_page = 12,
                q = "brush"
            }
        };
        _mockRepository.Setup(r => r.SearchAsync(searchRequest)).ReturnsAsync(expectedViewModel);
    
        // Act
        var result = await _productService.SearchAsync(searchRequest);
    
        // Assert
        result.Should().BeEquivalentTo(expectedViewModel);
        _mockRepository.Verify(r => r.SearchAsync(searchRequest), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmptyResult_WhenNoProductsMatch()
    {
        // Arrange
        var searchRequest = new SearchRequest 
        { 
            q = "nonexistent",
            filter_by = "{\"categories\": [\"019961f6-16b2-7aaf-8543-457fe1dbf084\"]}",
            sort_by = null,
            page = 1, 
            per_page = 12, 
        };
        var expectedViewModel = new PaginationViewModel<Product>
        {
            found = 0,
            page = 1,
            hits = [],
            request_params = new RequestParams
            {
                per_page = 12,
                q = "nonexistent"
            }
        };
        _mockRepository.Setup(r => r.SearchAsync(searchRequest)).ReturnsAsync(expectedViewModel);
    
        // Act
        var result = await _productService.SearchAsync(searchRequest);
    
        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.hits);
        Assert.Equal(0, result.found);
        _mockRepository.Verify(r => r.SearchAsync(searchRequest), Times.Once);
    }
}
