using Moq;
using Orders.Abstractions;
using Orders.Handlers;
using Orders.Models;

namespace Orders.Tests;

public class OrderCreatedHandlerTests
{
    private readonly Mock<INotificationSender> _sender = new();
    private readonly OrderCreatedHandler _sut;

    public OrderCreatedHandlerTests() => _sut = new(_sender.Object);

    [Fact]
    public async Task HandleAsync_ValidMessage_SendsNotification()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var message = new OrderCreated(orderId);

        // Act
        await _sut.HandleAsync(message);

        // Assert
        _sender.Verify(
            s => s.SendAsync(It.Is<string>(text => text.Contains(orderId.ToString()))),
            Times.Once());
    }

    [Fact]
    public async Task HandleAsync_EmptyOrderId_Throws()
    {
        // Arrange
        var message = new OrderCreated(Guid.Empty);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<ArgumentException>(() => _sut.HandleAsync(message));
        Assert.Contains("OrderId", ex.Message);
        _sender.Verify(s => s.SendAsync(It.IsAny<string>()), Times.Never());
    }
}
