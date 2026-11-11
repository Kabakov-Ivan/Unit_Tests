using Orders.Models;

namespace Orders.Tests;

public class OrderTests
{
    // Готовый пример теста. Остальные тесты вы пишете по аналогии.
    [Fact]
    public void Total_TwoItems_ReturnsSum()
    {
        // Arrange
        var order = new Order();
        order.Add("Книга", 300m, 2);
        order.Add("Ручка", 50m, 1);

        // Act
        var total = order.Total();

        // Assert
        Assert.Equal(650m, total);
    }

    [Fact]
    public void Total_EmptyOrder_ReturnsZero()
    {
        // Arrange
        var order = new Order();

        // Act
        var total = order.Total();

        // Assert
        Assert.Equal(0m, total);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Add_InvalidName_Throws(string? invalidName)
    {
        // Arrange
        var order = new Order();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => order.Add(invalidName!, 100m, 1));
    }

    [Fact]
    public void Constructor_WithItems_SameTotalAsAdd()
    {
        // Arrange
        var items = new List<OrderItem>
        {
            new("Книга", 300m, 2),
            new("Ручка", 50m, 1)
        };

        // Act
        var order = new Order(items);

        // Assert
        Assert.Equal(650m, order.Total());
    }
}
