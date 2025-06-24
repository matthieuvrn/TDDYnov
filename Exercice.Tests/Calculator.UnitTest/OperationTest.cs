using FluentAssertions;
using Xunit;

namespace Calculator.Test;

public class OperationTest
{
    private readonly Operation _operation = new();

    #region Add Tests

    [Fact]
    public void Add_WithTwoPositiveNumbers_ShouldReturnSum()
    {
        // Arrange
        int a = 5;
        int b = 3;

        // Act
        int result = _operation.Add(a, b);

        // Assert
        result.Should().Be(8);
    }

    [Theory]
    [InlineData(10, 5, 15)]
    [InlineData(-5, 3, -2)]
    [InlineData(-10, -5, -15)]
    [InlineData(0, 5, 5)]
    [InlineData(0, 0, 0)]
    [InlineData(int.MaxValue - 1, 1, int.MaxValue)]
    public void Add_WithVariousNumbers_ShouldReturnCorrectSum(int a, int b, int expected)
    {
        // Act
        int result = _operation.Add(a, b);

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region Subtract Tests

    [Fact]
    public void Subtract_WithTwoPositiveNumbers_ShouldReturnSubtraction()
    {
        // Arrange
        int a = 5;
        int b = 3;

        // Act
        int result = _operation.Subtract(a, b);

        // Assert
        result.Should().Be(2);
    }

    [Theory]
    [InlineData(10, 5, 5)]
    [InlineData(5, 10, -5)]
    [InlineData(-5, 3, -8)]
    [InlineData(-10, -5, -5)]
    [InlineData(0, 5, -5)]
    [InlineData(5, 0, 5)]
    [InlineData(0, 0, 0)]
    public void Subtract_WithVariousNumbers_ShouldReturnCorrectDifference(int a, int b, int expected)
    {
        // Act
        int result = _operation.Subtract(a, b);

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region Multiply Tests

    [Theory]
    [InlineData(5, 3, 15)]
    [InlineData(-5, 3, -15)]
    [InlineData(-5, -3, 15)]
    [InlineData(0, 5, 0)]
    [InlineData(5, 0, 0)]
    [InlineData(1, 100, 100)]
    [InlineData(-1, 100, -100)]
    public void Multiply_WithVariousNumbers_ShouldReturnCorrectProduct(int a, int b, int expected)
    {
        // Act
        int result = _operation.Multiply(a, b);

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region Divide Tests

    [Fact]
    public void Divide_WithNumberByZero_ShouldThrowException()
    {
        // Arrange
        int a = 5;
        int b = 0;

        // Act & Assert
        _operation.Invoking(op => op.Divide(a, b))
            .Should().Throw<DivideByZeroException>()
            .WithMessage("Division by zero is not allowed");
    }

    [Theory]
    [InlineData(10, 2, 5.0)]
    [InlineData(7, 2, 3.5)]
    [InlineData(-10, 2, -5.0)]
    [InlineData(10, -2, -5.0)]
    [InlineData(-10, -2, 5.0)]
    [InlineData(0, 5, 0.0)]
    public void Divide_WithValidNumbers_ShouldReturnCorrectQuotient(int a, int b, double expected)
    {
        // Act
        double result = _operation.Divide(a, b);

        // Assert
        result.Should().BeApproximately(expected, 0.001);
    }

    #endregion

    #region Power Tests

    [Theory]
    [InlineData(2, 3, 8)]
    [InlineData(5, 0, 1)]
    [InlineData(0, 5, 0)]
    [InlineData(1, 100, 1)]
    [InlineData(-2, 3, -8)]
    [InlineData(-2, 2, 4)]
    [InlineData(10, 1, 10)]
    public void Power_WithValidNumbers_ShouldReturnCorrectResult(int a, int b, int expected)
    {
        // Act
        int result = _operation.Power(a, b);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public void Power_WithNegativeExponent_ShouldThrowException()
    {
        // Act & Assert
        _operation.Invoking(op => op.Power(2, -1))
            .Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("Exponent cannot be negative*");
    }

    #endregion

    #region Square Tests

    [Theory]
    [InlineData(5, 25)]
    [InlineData(-5, 25)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(10, 100)]
    public void Square_WithVariousNumbers_ShouldReturnCorrectSquare(int a, int expected)
    {
        // Act
        int result = _operation.Square(a);

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region Cube Tests

    [Theory]
    [InlineData(3, 27)]
    [InlineData(-3, -27)]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(2, 8)]
    [InlineData(5, 125)]
    public void Cube_WithVariousNumbers_ShouldReturnCorrectCube(int a, int expected)
    {
        // Act
        int result = _operation.Cube(a);

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region Factorial Tests

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 1)]
    [InlineData(2, 2)]
    [InlineData(3, 6)]
    [InlineData(4, 24)]
    [InlineData(5, 120)]
    public void Factorial_WithValidNumbers_ShouldReturnCorrectFactorial(int a, int expected)
    {
        // Act
        int result = _operation.Factorial(a);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-5)]
    [InlineData(-100)]
    public void Factorial_WithNegativeNumber_ShouldThrowException(int a)
    {
        // Act & Assert
        _operation.Invoking(op => op.Factorial(a))
            .Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("Factorial cannot be calculated for negative numbers*");
    }

    #endregion

    #region SquareRoot Tests

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(4, 2)]
    [InlineData(9, 3)]
    [InlineData(16, 4)]
    [InlineData(25, 5)]
    public void SquareRoot_WithValidNumbers_ShouldReturnCorrectSquareRoot(int a, int expected)
    {
        // Act
        int result = _operation.SquareRoot(a);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-5)]
    [InlineData(-100)]
    public void SquareRoot_WithNegativeNumber_ShouldThrowException(int a)
    {
        // Act & Assert
        _operation.Invoking(op => op.SquareRoot(a))
            .Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage("Square root cannot be calculated for negative numbers*");
    }

    #endregion

    #region CubeRoot Tests

    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(8, 2)]
    [InlineData(27, 3)]
    [InlineData(-8, -2)]
    [InlineData(-27, -3)]
    [InlineData(64, 4)]
    [InlineData(125, 5)]
    public void CubeRoot_WithVariousNumbers_ShouldReturnCorrectCubeRoot(int a, int expected)
    {
        // Act
        int result = _operation.CubeRoot(a);

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region IsEven Tests

    [Theory]
    [InlineData(2, true)]
    [InlineData(4, true)]
    [InlineData(0, true)]
    [InlineData(-2, true)]
    [InlineData(-4, true)]
    [InlineData(100, true)]
    public void IsEven_WithEvenNumbers_ShouldReturnTrue(int number, bool expected)
    {
        // Act
        bool result = _operation.IsEven(number);

        // Assert
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData(1, false)]
    [InlineData(3, false)]
    [InlineData(-1, false)]
    [InlineData(-3, false)]
    [InlineData(99, false)]
    [InlineData(101, false)]
    public void IsEven_WithOddNumbers_ShouldReturnFalse(int number, bool expected)
    {
        // Act
        bool result = _operation.IsEven(number);

        // Assert
        result.Should().Be(expected);
    }

    #endregion
}