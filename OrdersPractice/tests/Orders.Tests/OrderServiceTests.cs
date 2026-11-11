using Moq;
using Orders.Abstractions;
using Orders.Models;
using Orders.Services;

namespace Orders.Tests;

public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _repo = new();
    private readonly Mock<IMessagePublisher> _pub = new();
    private readonly OrderService _sut;

    public OrderServiceTests() => _sut = new(_repo.Object, _pub.Object, new DiscountCalculator());

    private static NewOrder Cmd() => new(new List<OrderItem> { new("Книга", 300m, 2) });

    [Fact]
    public async Task CreateAsync_ValidOrder_SavesOrder()
    {
        // Arrange
        var cmd = Cmd();

        // Act
        await _sut.CreateAsync(cmd);

        // Assert
        _repo.Verify(r => r.SaveAsync(It.IsAny<Order>()), Times.Once());
    }

    [Fact]
    public async Task CreateAsync_ValidOrder_PublishesEventWithSameId()
    {
        // Arrange
        var cmd = Cmd();

        // Act
        var id = await _sut.CreateAsync(cmd);

        // Assert
        _pub.Verify(p => p.PublishAsync(It.Is<OrderCreated>(e => e.OrderId == id)), Times.Once());
    }

    [Fact]
    public async Task CreateAsync_SaveFails_DoesNotPublish()
    {
        // Arrange
        var cmd = Cmd();
        _repo.Setup(r => r.SaveAsync(It.IsAny<Order>()))
             .ThrowsAsync(new InvalidOperationException("DB error"));

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateAsync(cmd));
        _pub.Verify(p => p.PublishAsync(It.IsAny<OrderCreated>()), Times.Never());
    }


    [Fact]
    public async Task CreateAsync_EmptyItems_ThrowsAndDoesNotSave()
    {
        // Arrange
        var emptyCmd = new NewOrder(new List<OrderItem>());

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateAsync(emptyCmd));
        _repo.Verify(r => r.SaveAsync(It.IsAny<Order>()), Times.Never());
    }


    [Fact]
    public async Task GetPayableAsync_TotalBelowThreshold_NoDiscount()
    {
        // Arrange
        var order = new Order();
        order.Add("Книга", 500m, 1);
        _repo.Setup(r => r.FindAsync(order.Id)).ReturnsAsync(order);

        // Act
        var payable = await _sut.GetPayableAsync(order.Id);

        // Assert
        Assert.Equal(500m, payable);
    }

    [Fact]
    public async Task GetPayableAsync_TotalAboveThreshold_AppliesDiscount()
    {
        // Arrange
        var order = new Order();
        order.Add("Ноутбук", 1500m, 1);
        _repo.Setup(r => r.FindAsync(order.Id)).ReturnsAsync(order);

        // Act
        var payable = await _sut.GetPayableAsync(order.Id);

        // Assert
        Assert.Equal(1350m, payable);
    }

    [Fact]
    public async Task GetPayableAsync_OrderNotFound_Throws()
    {
        // Arrange
        var unknownId = Guid.NewGuid();
        _repo.Setup(r => r.FindAsync(unknownId)).ReturnsAsync((Order?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetPayableAsync(unknownId));
    }
}