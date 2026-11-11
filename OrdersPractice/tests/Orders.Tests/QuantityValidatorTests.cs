using Orders.Validation;

namespace Orders.Tests;

public class QuantityValidatorTests
{
    [Theory]
    [InlineData(-1, false)]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(100, true)]
    [InlineData(101, false)]
    public void IsValidQty_ChecksBounds(int qty, bool expected)
    {
        // Act
        var result = QuantityValidator.IsValidQty(qty);

        // Assert
        Assert.Equal(expected, result);
    }
}
