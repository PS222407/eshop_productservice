using eshop_productservice.DTOs;
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
    private readonly Mock<IProductRepository> _mockProductRepository;
    private readonly Mock<ISearchRepository> _mockSearchRepository;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _mockProductRepository = new Mock<IProductRepository>();
        _mockSearchRepository = new Mock<ISearchRepository>();
        _productService = new ProductService(_mockProductRepository.Object, _mockSearchRepository.Object);
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
        _mockProductRepository.Setup(r => r.GetAsync(productId)).ReturnsAsync(expectedProduct);

        // Act
        var result = await _productService.GetAsync(productId);

        // Assert
        result.Should().BeEquivalentTo(expectedProduct);
        _mockProductRepository.Verify(r => r.GetAsync(productId), Times.Once);
    }

    [Fact]
    public async Task GetAsync_WithId_ReturnsNull_WhenProductNotFound()
    {
        // Arrange
        const string productId = "999";
        _mockProductRepository.Setup(r => r.GetAsync(productId)).ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.GetAsync(productId);

        // Assert
        Assert.Null(result);
        _mockProductRepository.Verify(r => r.GetAsync(productId), Times.Once);
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
            new()
            {
                Id = ids[0].ToString(), Name = "Laptop", PriceInCents = 99999, StarsTimesTen = 46,
                ImageUrl = "https://example.com/laptop.jpg"
            },
            new()
            {
                Id = ids[1].ToString(), Name = "Mouse", PriceInCents = 1999, StarsTimesTen = 40,
                ImageUrl = "https://example.com/mouse.jpg"
            }
        };
        _mockProductRepository.Setup(r => r.GetAsync(ids)).ReturnsAsync(expectedProducts);

        // Act
        var result = await _productService.GetAsync(ids);

        // Assert
        result.Should().BeEquivalentTo(expectedProducts);
        _mockProductRepository.Verify(r => r.GetAsync(ids), Times.Once);
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
        _mockProductRepository.Setup(r => r.GetAsync(ids)).ReturnsAsync(emptyList);

        // Act
        var result = await _productService.GetAsync(ids);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
        _mockProductRepository.Verify(r => r.GetAsync(ids), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_ReturnsPagedProducts()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            q = "brush",
            filter_by = "{\"CategoryId\": [\"019961f6-16b2-7aaf-8543-457fe1dbf084\"]}",
            sort_by = null,
            page = 1,
            per_page = 12
        };
        var expectedViewModel = new PaginationDto<Product>
        {
            found = 2,
            page = 1,
            hits =
            [
                new Product
                {
                    Id = "1", Name = "Gaming Laptop", PriceInCents = 129999, StarsTimesTen = 48,
                    ImageUrl = "https://example.com/laptop.jpg"
                },
                new Product
                {
                    Id = "2", Name = "Business Laptop", PriceInCents = 89999, StarsTimesTen = 46,
                    ImageUrl = "https://example.com/laptop.jpg"
                }
            ],
            request_params = new RequestParams
            {
                per_page = 12,
                q = "brush"
            }
        };
        _mockSearchRepository.Setup(r => r.Products(searchRequest)).ReturnsAsync(expectedViewModel);

        // Act
        var result = await _productService.SearchAsync(searchRequest);

        // Assert
        result.Should().BeEquivalentTo(expectedViewModel);
        _mockSearchRepository.Verify(r => r.Products(searchRequest), Times.Once);
    }

    [Fact]
    public async Task SearchAsync_ReturnsEmptyResult_WhenNoProductsMatch()
    {
        // Arrange
        var searchRequest = new SearchRequest
        {
            q = "nonexistent",
            filter_by = "{\"CategoryId\": [\"019961f6-16b2-7aaf-8543-457fe1dbf084\"]}",
            sort_by = null,
            page = 1,
            per_page = 12
        };
        var expectedViewModel = new PaginationDto<Product>
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
        _mockSearchRepository.Setup(r => r.Products(searchRequest)).ReturnsAsync(expectedViewModel);

        // Act
        var result = await _productService.SearchAsync(searchRequest);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.hits);
        Assert.Equal(0, result.found);
        _mockSearchRepository.Verify(r => r.Products(searchRequest), Times.Once);
    }
}