using FluentAssertions;
using Moq;
using Notification.Contracts;
using Notification.Services;

namespace Notification.UnitTest;

public class EmailServiceTest
{
    private readonly Mock<ILogger> _loggerMock;
    private readonly EmailService _emailService;

    public EmailServiceTest()
    {
        _loggerMock = new Mock<ILogger>();
        _emailService = new EmailService(_loggerMock.Object);
    }

    [Theory]
    [InlineData("test@example.com", true)]
    [InlineData("user.name+tag@domain.co.uk", true)]
    [InlineData("invalid-email", false)]
    [InlineData("test@", false)]
    [InlineData("@domain.com", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValidEmail_WithVariousFormats_ReturnsExpectedResult(string email, bool expected)
    {
        // Act
        var result = _emailService.IsValidEmail(email);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public async Task SendEmailAsync_WithValidParameters_ReturnsTrue()
    {
        // Arrange
        var to = "test@example.com";
        var subject = "Test Subject";
        var body = "Test Body";

        // Act
        var result = await _emailService.SendEmailAsync(to, subject, body);

        // Assert
        result.Should().BeTrue();
        _loggerMock.Verify(x => x.LogInfo(It.Is<string>(msg => msg.Contains("sent successfully"))), Times.Once);
    }


    [Theory]
    [InlineData("", "subject", "body")]
    [InlineData("test@example.com", "", "body")]
    [InlineData("test@example.com", null, "body")]
    public async Task SendEmailAsync_WithMissingParameters_ReturnsFalse(string to, string subject, string body)
    {
        // Act
        var result = await _emailService.SendEmailAsync(to, subject, body);

        // Assert
        result.Should().BeFalse();
        _loggerMock.Verify(x => x.LogWarning(It.Is<string>(msg => msg.Contains("missing required parameters"))), Times.Once);
    }

    [Fact]
    public void SendWelcomeEmail_WithValidParameters_SendsEmail()
    {
        // Arrange
        var email = "test@example.com";
        var name = "John Doe";

        // Act
        _emailService.SendWelcomeEmail(email, name);

        // Assert
        _loggerMock.Verify(x => x.LogInfo(It.Is<string>(msg => msg.Contains("Welcome email process completed"))), Times.Once);
    }

    [Fact]
    public void SendWelcomeEmail_WithInvalidEmail_LogsWarning()
    {
        // Arrange
        var invalidEmail = "invalid-email";
        var name = "John Doe";

        // Act
        _emailService.SendWelcomeEmail(invalidEmail, name);

        // Assert
        _loggerMock.Verify(x => x.LogWarning(It.Is<string>(msg => msg.Contains("Invalid email format"))), Times.Once);
    }
}