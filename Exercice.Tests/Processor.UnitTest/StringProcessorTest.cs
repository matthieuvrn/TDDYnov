using Processor.Services;
using FluentAssertions;

namespace Processor.UnitTest;

public class StringProcessorTest
{
    private readonly StringProcessor _processor = new();

    #region Reverse Tests

    [Theory]
    [InlineData("hello", "olleh")]
    [InlineData("", "")]
    [InlineData("a", "a")]
    [InlineData("12345", "54321")]
    [InlineData("Hello World!", "!dlroW olleH")]
    [Trait("Category", "StringManipulation")]
    [Trait("Method", "Reverse")]
    public void Reverse_VariousInputs_ReturnsReversedString(string input, string expected)
    {
        // Arrange & Act
        string result = _processor.Reverse(input);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    [Trait("Category", "StringManipulation")]
    [Trait("Method", "Reverse")]
    public void Reverse_WithNullInput_ReturnsEmptyString()
    {
        // Arrange
        string input = null;

        // Act
        string result = _processor.Reverse(input);

        // Assert
        result.Should().Be(string.Empty);
    }

    #endregion

    #region IsPalindrome Tests

    [Theory]
    [InlineData("radar", true)]
    [InlineData("hello", false)]
    [InlineData("A man a plan a canal Panama", true)]
    [InlineData("", true)]
    [InlineData("a", true)]
    [InlineData("Aa", true)]
    [InlineData("race a car", false)]
    [InlineData("Was it a car or a cat I saw?", true)]
    [Trait("Category", "StringValidation")]
    [Trait("Method", "IsPalindrome")]
    public void IsPalindrome_VariousInputs_ReturnsCorrectResult(string input, bool expected)
    {
        // Arrange & Act
        bool isPalindrome = _processor.IsPalindrome(input);

        // Assert
        isPalindrome.Should().Be(expected);
    }

    [Fact]
    [Trait("Category", "StringValidation")]
    [Trait("Method", "IsPalindrome")]
    public void IsPalindrome_WithNullInput_ReturnsTrue()
    {
        // Arrange
        string input = null;

        // Act
        bool result = _processor.IsPalindrome(input);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData("   ", true)] // Espaces seulement
    [InlineData("!@#$%", true)] // Caractères spéciaux seulement
    [InlineData("12321", true)] // Chiffres palindrome
    [InlineData("12345", false)] // Chiffres non palindrome
    [Trait("Category", "StringValidation")]
    [Trait("Method", "IsPalindrome")]
    public void IsPalindrome_WithSpecialCases_ReturnsCorrectResult(string input, bool expected)
    {
        // Arrange & Act
        bool result = _processor.IsPalindrome(input);

        // Assert
        result.Should().Be(expected);
    }

    #endregion

    #region CountWords Tests

    [Theory]
    [InlineData("Hello world", 2)]
    [InlineData("", 0)]
    [InlineData("   ", 0)]
    [InlineData("word", 1)]
    [InlineData("Hello    world    test", 3)] // Espaces multiples
    [InlineData("Hello\tworld\ntest", 3)] // Différents séparateurs
    [InlineData("Hello,world.test!", 1)] // Pas de séparateurs d'espaces
    [Trait("Category", "StringAnalysis")]
    [Trait("Method", "CountWords")]
    public void CountWords_VariousInputs_ReturnsCorrectCount(string input, int expected)
    {
        // Arrange & Act
        int result = _processor.CountWords(input);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    [Trait("Category", "StringAnalysis")]
    [Trait("Method", "CountWords")]
    public void CountWords_WithNullInput_ReturnsZero()
    {
        // Arrange
        string input = null;

        // Act
        int result = _processor.CountWords(input);

        // Assert
        result.Should().Be(0);
    }

    #endregion

    #region Capitalize Tests

    [Theory]
    [InlineData("hello", "Hello")]
    [InlineData("HELLO", "HELLO")]
    [InlineData("hELLO", "HELLO")]
    [InlineData("", "")]
    [InlineData("a", "A")]
    [InlineData("hello world", "Hello world")] // Seul le premier caractère
    [InlineData("123hello", "123hello")] // Premier caractère non-lettre
    [Trait("Category", "StringFormatting")]
    [Trait("Method", "Capitalize")]
    public void Capitalize_VariousInputs_ReturnsCapitalizedString(string input, string expected)
    {
        // Arrange & Act
        string result = _processor.Capitalize(input);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    [Trait("Category", "StringFormatting")]
    [Trait("Method", "Capitalize")]
    public void Capitalize_WithNullInput_ReturnsEmptyString()
    {
        // Arrange
        string input = null;

        // Act
        string result = _processor.Capitalize(input);

        // Assert
        result.Should().Be(string.Empty);
    }


    #endregion
}