using Orders.Models;
using Orders.Repositories;

namespace Orders.Tests;

public class InMemoryOrderRepositoryTests
{
    private readonly InMemoryOrderRepository _sut = new();

    [Fact]
    public async Task SaveAsync_ThenFindAsync_ReturnsSameOrder()
    {
        // Arrange
        var order = new Order();
        order.Add("Книга", 300m, 1);

        // Act
        await _sut.SaveAsync(order);
        var found = await _sut.FindAsync(order.Id);

        // Assert
        Assert.NotNull(found);
        Assert.Equal(order.Id, found.Id);
    }

    [Fact]
    public async Task FindAsync_UnknownId_ReturnsNull()
    {
        // Arrange
        var unknownId = Guid.NewGuid();

        // Act
        var found = await _sut.FindAsync(unknownId);

        // Assert
        Assert.Null(found);
    }

    [Fact]
    public async Task SaveAsync_Null_Throws()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _sut.SaveAsync(null!));
    }
}