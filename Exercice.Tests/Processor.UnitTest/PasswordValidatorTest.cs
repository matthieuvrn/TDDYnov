using Processor.Services;
using FluentAssertions;

namespace Processor.UnitTest;

public class PasswordValidatorTest
{
    private readonly PasswordValidator _validator = new();

    [Theory]
    [InlineData("Password123!", true)]
    [InlineData("password123!", false)] 
    [InlineData("PASSWORD123!", false)] 
    [InlineData("Password!", false)]   
    [InlineData("Password123", false)]    
    [InlineData("", false)]          
    [InlineData("Password 123!", false)] 
    public void Validate_WithDifferentPassword_ReturnsCorrectResult(string password, bool expectedValid)
    {
        // Arrange & Act
        var result = _validator.Validate(password);

        // Assert
        result.IsValid.Should().Be(expectedValid);
    }

    [Fact]
    public void Validate_WithNoUppercaseLetter_ReturnCorrectErrorMessage()
    {
        // Arrange
        var password = "password123!";

        // Act
        var result = _validator.Validate(password);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Le mot de passe doit contenir au moins une majuscule");
    }

    [Fact]
    public void Validate_WithNoLowercaseLetter_ReturnCorrectErrorMessage()
    {
        // Arrange
        var password = "PASSWORD123!";

        // Act
        var result = _validator.Validate(password);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Le mot de passe doit contenir au moins une minuscule");
    }

    [Fact]
    public void Validate_WithNoDigitLetter_ReturnCorrectErrorMessage()
    {
        // Arrange
        var password = "Password!";

        // Act
        var result = _validator.Validate(password);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Le mot de passe doit contenir au moins un chiffre");
    }

    [Fact]
    public void Validate_WithPasswordTooShort_ReturnCorrectErrorMessage()
    {
        // Arrange
        var password = "Pass1!";

        // Act
        var result = _validator.Validate(password);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Le mot de passe doit contenir au moins 8 caractères");
    }

    [Fact]
    public void Validate_WithEmptyPassword_ReturnCorrectErrorMessage()
    {
        // Arrange
        var password = "";

        // Act
        var result = _validator.Validate(password);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Le mot de passe ne peut pas être vide");
    }

    [Fact]
    public void Validate_WithNullPassword_ReturnCorrectErrorMessage()
    {
        // Arrange
        string password = null;

        // Act
        var result = _validator.Validate(password);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Le mot de passe ne peut pas être vide");
    }

    [Fact]
    public void Validate_WithNoSpecialCharacter_ReturnCorrectErrorMessage()
    {
        // Arrange
        var password = "Password123";

        // Act
        var result = _validator.Validate(password);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Le mot de passe doit contenir au moins un caractère spécial (!@#$%^&*()_+-=[]{}|;':\",./<>?)");
    }

    [Fact]
    public void Validate_WithPasswordContainingSpaces_ReturnCorrectErrorMessage()
    {
        // Arrange
        var password = "Password 123!";

        // Act
        var result = _validator.Validate(password);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Le mot de passe ne peut pas contenir d'espaces");
    }

    [Fact]
    public void Validate_WithPasswordTooLong_ReturnCorrectErrorMessage()
    {
        // Arrange
        var password = new string('a', 51) + "A1!"; // 54 caractères

        // Act
        var result = _validator.Validate(password);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Le mot de passe ne peut pas dépasser 50 caractères");
    }

}