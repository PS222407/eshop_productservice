using eshop_productservice.Interfaces;
using eshop_productservice.Listeners;
using Moq;

namespace UnitTests;

public class OrderListenerTests
{
    private readonly OrderListener _listener;
    private readonly Mock<IProductService> _productServiceMock;

    public OrderListenerTests()
    {
        _productServiceMock = new Mock<IProductService>();
        _listener = new OrderListener(_productServiceMock.Object);
    }

    [Fact]
    public async Task NewOrderCreated_ValidData_CallsDecreaseStockBy()
    {
        // Arrange
        const string orderJson = @"
        {
            ""Products"": [
                { ""Id"": ""product 1"", ""Quantity"": 2 },
                { ""Id"": ""product 2"", ""Quantity"": 5 }
            ]
        }";

        // Act
        await _listener.NewOrderCreated(orderJson);

        // Assert
        _productServiceMock.Verify(p => p.DecreaseStockBy("product 1", 2), Times.Once);
        _productServiceMock.Verify(p => p.DecreaseStockBy("product 2", 5), Times.Once);
    }

    [Fact]
    public async Task NewOrderCreated_InvalidData_LogsError()
    {
        // Arrange
        const string invalidJson = "not a valid json";

        // Act
        await _listener.NewOrderCreated(invalidJson);

        // Assert
        _productServiceMock.Verify(p => p.DecreaseStockBy(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task NewOrderCreated_NullOrder_LogsError()
    {
        // Arrange
        const string nullJson = "null";

        // Act
        await _listener.NewOrderCreated(nullJson);

        // Assert
        _productServiceMock.Verify(p => p.DecreaseStockBy(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
    }
}