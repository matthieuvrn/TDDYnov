using Learning;

namespace LearningUnitTest;

public class NumberListTest
{
    [Fact]
    public void Add_Number_IncreasesCount()
    {
        // Arrange
        var list = new NumberList();

        // Act
        list.Add(10);

        // Assert
        Assert.Equal(1, list.Count());
        Assert.True(list.Contains(10));
    }

    [Fact]
    public void Remove_ExistingNumber_DecreasesCount()
    {
        // Arrange
        var list = new NumberList();
        list.Add(5);

        // Act
        bool removed = list.Remove(5);

        // Assert
        Assert.True(removed);
        Assert.Equal(0, list.Count());
        Assert.False(list.Contains(5));
    }

    [Fact]
    public void Remove_NonExistingNumber_ReturnsFalse()
    {
        // Arrange
        var list = new NumberList();
        list.Add(1);
        list.Add(2);
        list.Add(3);

        // Act
        bool removed = list.Remove(99);

        // Assert
        Assert.False(removed);
        Assert.Equal(3, list.Count());
    }

    [Fact]
    public void GetMax_WithNumbers_ReturnsMaximum()
    {
        // Arrange
        var list = new NumberList();
        list.Add(5);
        list.Add(10);
        list.Add(3);
        list.Add(8);

        // Act
        int max = list.Max();

        // Assert
        Assert.Equal(10, max);
    }

    [Fact]
    public void GetAverage_WithNumbers_ReturnsCorrectAverage()
    {
        // Arrange
        var list = new NumberList();
        list.Add(2);
        list.Add(4);
        list.Add(6);
        list.Add(8);

        // Act
        double average = list.Average();

        // Assert
        Assert.Equal(5.0, average);
    }

    [Fact]
    public void Clear_RemovesAllNumbers()
    {
        // Arrange
        var list = new NumberList();
        list.Add(1);
        list.Add(2);
        list.Add(3);

        // Act
        list.Clear();

        // Assert
        Assert.Equal(0, list.Count());
    }

    [Fact]
    public void Sum_WithNumbers_ReturnsCorrectSum()
    {
        // Arrange
        var list = new NumberList();
        list.Add(1);
        list.Add(2);
        list.Add(3);
        list.Add(4);

        // Act
        int sum = list.Sum();

        // Assert
        Assert.Equal(10, sum);
    }

    [Fact]
    public void Min_WithNumbers_ReturnsMinimum()
    {
        // Arrange
        var list = new NumberList();
        list.Add(5);
        list.Add(10);
        list.Add(3);
        list.Add(8);

        // Act
        int min = list.Min();

        // Assert
        Assert.Equal(3, min);
    }

    [Fact]
    public void Max_EmptyList_ThrowsException()
    {
        // Arrange
        var list = new NumberList();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => list.Max());
    }

    [Fact]
    public void Min_EmptyList_ThrowsException()
    {
        // Arrange
        var list = new NumberList();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => list.Min());
    }

    [Fact]
    public void Average_EmptyList_ThrowsException()
    {
        // Arrange
        var list = new NumberList();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => list.Average());
    }
}