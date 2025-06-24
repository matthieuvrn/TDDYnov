using Learning;

namespace LearningUnitTest;

public class MathHelperTest
{
    private readonly MathHelper _mathHelper;

    public MathHelperTest()
    {
        _mathHelper = new MathHelper();
    }

    [Fact]
    public void IsEven_EvenNumber_ReturnsTrue()
    {
        // Arrange
        int evenNumber = 4;

        // Act
        bool result = _mathHelper.IsEven(evenNumber);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsEven_OddNumber_ReturnsFalse()
    {
        // Arrange
        int oddNumber = 5;

        // Act
        bool result = _mathHelper.IsEven(oddNumber);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsEven_Zero_ReturnsTrue()
    {
        // Arrange
        int zero = 0;

        // Act
        bool result = _mathHelper.IsEven(zero);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsEven_NegativeEvenNumber_ReturnsTrue()
    {
        // Arrange
        int negativeEven = -6;

        // Act
        bool result = _mathHelper.IsEven(negativeEven);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsEven_NegativeOddNumber_ReturnsFalse()
    {
        // Arrange
        int negativeOdd = -7;

        // Act
        bool result = _mathHelper.IsEven(negativeOdd);

        // Assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(2, true)]
    [InlineData(4, true)]
    [InlineData(100, true)]
    [InlineData(1, false)]
    [InlineData(3, false)]
    [InlineData(99, false)]
    [InlineData(-2, true)]
    [InlineData(-4, true)]
    [InlineData(-1, false)]
    [InlineData(-3, false)]
    public void IsEven_VariousNumbers_ReturnsExpectedResult(int number, bool expected)
    {
        // Act
        bool result = _mathHelper.IsEven(number);

        // Assert
        Assert.Equal(expected, result);
    }
}