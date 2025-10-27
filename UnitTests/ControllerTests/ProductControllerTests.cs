using eshop_productservice.Controllers;
using eshop_productservice.DTOs;
using eshop_productservice.Interfaces;
using eshop_productservice.Models;
using eshop_productservice.Requests;
using eshop_productservice.ViewModels;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace UnitTests.ControllerTests;

public class ProductControllerTests
{
    private readonly ProductController _controller;
    private readonly Mock<IProductService> _mockProductService;

    public ProductControllerTests()
    {
        _mockProductService = new Mock<IProductService>();
        _controller = new ProductController(_mockProductService.Object);
    }

    [Fact]
    public async Task Search_ReturnsPaginatedProducts()
    {
        // Arrange
        var request = new SearchRequest { q = "test", page = 1, per_page = 10 };
        var expected = new PaginationDto<Product>
        {
            found = 1,
            page = 1,
            hits =
            [
                new Product
                {
                    Id = "1", Name = "Test Product", PriceInCents = 1000, ImageUrl = "url", StarsTimesTen = 50
                }
            ],
            request_params = new RequestParams { per_page = 10, q = "test" }
        };
        _mockProductService.Setup(s => s.SearchAsync(request)).ReturnsAsync(expected);

        // Act
        var result = await _controller.Search(request);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var value = okResult.Value as PaginationViewModel<Product>;
        value.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Get_ReturnsProduct()
    {
        // Arrange
        var product = new Product
            { Id = "1", Name = "Test Product", PriceInCents = 1000, ImageUrl = "url", StarsTimesTen = 50 };
        _mockProductService.Setup(s => s.GetAsync("1")).ReturnsAsync(product);

        // Act
        var result = await _controller.Get("1");

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var value = okResult.Value as Product;
        value.Should().BeEquivalentTo(product);
    }

    [Fact]
    public async Task Get_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        _mockProductService.Setup(s => s.GetAsync("999")).ReturnsAsync((Product?)null);

        // Act
        var result = await _controller.Get("999");

        // Assert
        var notFoundResult = result.Result as NotFoundResult;
        notFoundResult.Should().NotBeNull();
    }

    [Fact]
    public async Task GetBatch_ReturnsProducts()
    {
        // Arrange
        var ids = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };
        var request = new ProductBatchRequest { Ids = ids };
        var products = new List<Product>
        {
            new()
            {
                Id = ids[0].ToString(), Name = "Product 1", PriceInCents = 100, ImageUrl = "url1", StarsTimesTen = 40
            },
            new()
            {
                Id = ids[1].ToString(), Name = "Product 2", PriceInCents = 200, ImageUrl = "url2", StarsTimesTen = 80
            }
        };
        _mockProductService.Setup(s => s.GetAsync(ids)).ReturnsAsync(products);

        // Act
        var result = await _controller.GetBatch(request);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var value = okResult.Value as List<Product>;
        value.Should().BeEquivalentTo(products);
    }

    [Fact]
    public async Task GetBatch_ReturnsBadRequest_WhenIdsEmpty()
    {
        // Arrange
        var request = new ProductBatchRequest { Ids = new List<Guid>() };

        // Act
        var result = await _controller.GetBatch(request);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.Value.Should().Be("At least one id must be provided.");
    }

    [Fact]
    public async Task ImportProducts_CallsService_AndReturnsOk()
    {
        // Arrange
        _mockProductService.Setup(s => s.ImportProducts()).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ImportProducts();

        // Assert
        var okResult = result as OkResult;
        okResult.Should().NotBeNull();
        _mockProductService.Verify(s => s.ImportProducts(), Times.Once);
    }
}