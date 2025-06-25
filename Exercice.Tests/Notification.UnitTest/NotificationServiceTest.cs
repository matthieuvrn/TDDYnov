// NotificationServiceTest.cs - Tests complets
using FluentAssertions;
using Moq;
using Notification.Contracts;
using Notification.Services;

namespace Notification.UnitTest;

public class NotificationServiceTest
{
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly Mock<ISmsService> _smsServiceMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly NotificationService _notificationService;

    public NotificationServiceTest()
    {
        _emailServiceMock = new Mock<IEmailService>();
        _smsServiceMock = new Mock<ISmsService>();
        _loggerMock = new Mock<ILogger>();
        _notificationService = new NotificationService(
            _emailServiceMock.Object,
            _smsServiceMock.Object,
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task SendWelcomeEmailAsync_ValidEmail_SendsEmailAndLogsSuccess()
    {
        // Arrange
        var email = "test@example.com";
        var userName = "John Doe";

        _emailServiceMock.Setup(x => x.IsValidEmail(email)).Returns(true);
        _emailServiceMock.Setup(x => x.SendEmailAsync(email, "Welcome!", It.IsAny<string>()))
                        .ReturnsAsync(true);

        // Act
        var result = await _notificationService.SendWelcomeEmailAsync(email, userName);

        // Assert
        result.Should().BeTrue();

        _emailServiceMock.Verify(x => x.IsValidEmail(email), Times.Once);
        _emailServiceMock.Verify(x => x.SendEmailAsync(email, "Welcome!",
                                 It.Is<string>(body => body.Contains(userName))), Times.Once);
        _loggerMock.Verify(x => x.LogInfo(It.Is<string>(msg => msg.Contains("sent successfully"))),
                          Times.Once);
    }

    [Fact]
    public async Task SendWelcomeEmailAsync_InvalidEmail_LogsWarningAndReturnsFalse()
    {
        // Arrange
        var invalidEmail = "invalid-email";
        var userName = "John Doe";

        _emailServiceMock.Setup(x => x.IsValidEmail(invalidEmail)).Returns(false);

        // Act
        var result = await _notificationService.SendWelcomeEmailAsync(invalidEmail, userName);

        // Assert
        result.Should().BeFalse();

        _emailServiceMock.Verify(x => x.IsValidEmail(invalidEmail), Times.Once);
        _emailServiceMock.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(),
                                 It.IsAny<string>()), Times.Never);
        _loggerMock.Verify(x => x.LogWarning(It.Is<string>(msg => msg.Contains("Invalid email"))),
                          Times.Once);
    }

    [Fact]
    public async Task SendWelcomeEmailAsync_EmailServiceThrowsException_LogsErrorAndReturnsFalse()
    {
        // Arrange
        var email = "test@example.com";
        var userName = "John Doe";
        var exceptionMessage = "Network error";

        _emailServiceMock.Setup(x => x.IsValidEmail(email)).Returns(true);
        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                        .ThrowsAsync(new Exception(exceptionMessage));

        // Act
        var result = await _notificationService.SendWelcomeEmailAsync(email, userName);

        // Assert
        result.Should().BeFalse();
        _loggerMock.Verify(x => x.LogError(It.Is<string>(msg => msg.Contains(exceptionMessage))),
                          Times.Once);
    }

    [Fact]
    public async Task SendWelcomeEmailAsync_EmailServiceFails_LogsErrorAndReturnsFalse()
    {
        // Arrange
        var email = "test@example.com";
        var userName = "John Doe";

        _emailServiceMock.Setup(x => x.IsValidEmail(email)).Returns(true);
        _emailServiceMock.Setup(x => x.SendEmailAsync(email, "Welcome!", It.IsAny<string>()))
                        .ReturnsAsync(false);

        // Act
        var result = await _notificationService.SendWelcomeEmailAsync(email, userName);

        // Assert
        result.Should().BeFalse();
        _loggerMock.Verify(x => x.LogError(It.Is<string>(msg => msg.Contains("Failed to send"))),
                          Times.Once);
    }

    [Fact]
    public async Task SendNotificationAsync_ValidEmailAndPhone_SendsBoth()
    {
        // Arrange
        var email = "test@example.com";
        var phone = "+1234567890";
        var message = "Test notification";

        _emailServiceMock.Setup(x => x.IsValidEmail(email)).Returns(true);
        _emailServiceMock.Setup(x => x.SendEmailAsync(email, "Notification", message))
                        .ReturnsAsync(true);

        _smsServiceMock.Setup(x => x.IsValidPhoneNumber(phone)).Returns(true);
        _smsServiceMock.Setup(x => x.SendSmsAsync(phone, message)).ReturnsAsync(true);

        // Act
        var result = await _notificationService.SendNotificationAsync(email, phone, message);

        // Assert
        result.Should().BeTrue();

        _emailServiceMock.Verify(x => x.SendEmailAsync(email, "Notification", message), Times.Once);
        _smsServiceMock.Verify(x => x.SendSmsAsync(phone, message), Times.Once);
    }

    [Fact]
    public async Task SendNotificationAsync_EmailFailsButSmsSucceeds_ReturnsTrue()
    {
        // Arrange
        var email = "test@example.com";
        var phone = "+1234567890";
        var message = "Test notification";

        _emailServiceMock.Setup(x => x.IsValidEmail(email)).Returns(true);
        _emailServiceMock.Setup(x => x.SendEmailAsync(email, "Notification", message))
                        .ReturnsAsync(false); // Email échoue

        _smsServiceMock.Setup(x => x.IsValidPhoneNumber(phone)).Returns(true);
        _smsServiceMock.Setup(x => x.SendSmsAsync(phone, message))
                      .ReturnsAsync(true); // SMS réussit

        // Act
        var result = await _notificationService.SendNotificationAsync(email, phone, message);

        // Assert
        result.Should().BeTrue("because SMS succeeded even though email failed");

        _emailServiceMock.Verify(x => x.SendEmailAsync(email, "Notification", message), Times.Once);
        _smsServiceMock.Verify(x => x.SendSmsAsync(phone, message), Times.Once);
    }

    [Fact]
    public async Task SendNotificationAsync_BothEmailAndSmsFail_ReturnsFalse()
    {
        // Arrange
        var email = "test@example.com";
        var phone = "+1234567890";
        var message = "Test notification";

        _emailServiceMock.Setup(x => x.IsValidEmail(email)).Returns(true);
        _emailServiceMock.Setup(x => x.SendEmailAsync(email, "Notification", message))
                        .ReturnsAsync(false);

        _smsServiceMock.Setup(x => x.IsValidPhoneNumber(phone)).Returns(true);
        _smsServiceMock.Setup(x => x.SendSmsAsync(phone, message))
                      .ReturnsAsync(false);

        // Act
        var result = await _notificationService.SendNotificationAsync(email, phone, message);

        // Assert
        result.Should().BeFalse("because both email and SMS failed");
    }

    [Fact]
    public async Task SendNotificationAsync_EmptyEmailValidPhone_SendsOnlySms()
    {
        // Arrange
        var email = "";
        var phone = "+1234567890";
        var message = "Test notification";

        _smsServiceMock.Setup(x => x.IsValidPhoneNumber(phone)).Returns(true);
        _smsServiceMock.Setup(x => x.SendSmsAsync(phone, message)).ReturnsAsync(true);

        // Act
        var result = await _notificationService.SendNotificationAsync(email, phone, message);

        // Assert
        result.Should().BeTrue();

        _emailServiceMock.Verify(x => x.IsValidEmail(It.IsAny<string>()), Times.Never);
        _emailServiceMock.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        _smsServiceMock.Verify(x => x.SendSmsAsync(phone, message), Times.Once);
    }

    [Fact]
    public async Task SendNotificationAsync_WithCallback_CapturesParameters()
    {
        // Arrange
        var email = "test@example.com";
        var phone = "+1234567890";
        var message = "Test notification";

        var capturedEmailParams = new List<(string to, string subject, string body)>();
        var capturedSmsParams = new List<(string phoneNumber, string message)>();

        _emailServiceMock.Setup(x => x.IsValidEmail(email)).Returns(true);
        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                        .Callback<string, string, string>((to, subject, body) =>
                            capturedEmailParams.Add((to, subject, body)))
                        .ReturnsAsync(true);

        _smsServiceMock.Setup(x => x.IsValidPhoneNumber(phone)).Returns(true);
        _smsServiceMock.Setup(x => x.SendSmsAsync(It.IsAny<string>(), It.IsAny<string>()))
                      .Callback<string, string>((phoneNumber, msg) =>
                          capturedSmsParams.Add((phoneNumber, msg)))
                      .ReturnsAsync(true);

        // Act
        await _notificationService.SendNotificationAsync(email, phone, message);

        // Assert
        capturedEmailParams.Should().HaveCount(1);
        capturedEmailParams[0].to.Should().Be(email);
        capturedEmailParams[0].subject.Should().Be("Notification");
        capturedEmailParams[0].body.Should().Be(message);

        capturedSmsParams.Should().HaveCount(1);
        capturedSmsParams[0].phoneNumber.Should().Be(phone);
        capturedSmsParams[0].message.Should().Be(message);
    }

    [Theory]
    [InlineData("test@domain.com", true)]
    [InlineData("invalid-email", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public async Task SendWelcomeEmailAsync_DifferentEmailFormats_HandlesCorrectly(string email, bool shouldSendEmail)
    {
        // Arrange
        var userName = "Test User";

        _emailServiceMock.Setup(x => x.IsValidEmail(It.IsAny<string>()))
                        .Returns<string>(e => !string.IsNullOrWhiteSpace(e) && e.Contains("@") && e.Contains("."));

        if (shouldSendEmail)
        {
            _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                            .ReturnsAsync(true);
        }

        // Act
        var result = await _notificationService.SendWelcomeEmailAsync(email, userName);

        // Assert
        if (shouldSendEmail)
        {
            result.Should().BeTrue();
            _emailServiceMock.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }
        else
        {
            result.Should().BeFalse();
            _emailServiceMock.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }

    [Fact]
    public async Task SendWelcomeEmailAsync_UsingItIsRegex_ValidatesEmailPattern()
    {
        // Arrange
        var email = "user123@domain.co.uk";
        var userName = "Test User";

        _emailServiceMock.Setup(x => x.IsValidEmail(It.IsRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")))
                        .Returns(true);
        _emailServiceMock.Setup(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                        .ReturnsAsync(true);

        // Act
        var result = await _notificationService.SendWelcomeEmailAsync(email, userName);

        // Assert
        result.Should().BeTrue();
        _emailServiceMock.Verify(x => x.IsValidEmail(It.IsRegex(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$")), Times.Once);
    }
}