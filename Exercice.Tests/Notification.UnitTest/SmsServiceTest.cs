using FluentAssertions;
using Moq;
using Notification.Contracts;
using Notification.Services;

namespace Notification.UnitTest;

public class SmsServiceTest
{
    private readonly Mock<ILogger> _loggerMock;
    private readonly SmsService _smsService;

    public SmsServiceTest()
    {
        _loggerMock = new Mock<ILogger>();
        _smsService = new SmsService(_loggerMock.Object);
    }

    [Theory]
    [InlineData("+33123456789", true)]
    [InlineData("0123456789", true)]
    [InlineData("123-456-7890", true)]
    [InlineData("123 456 7890", true)]
    [InlineData("123", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValidPhoneNumber_WithVariousFormats_ReturnsExpectedResult(string phoneNumber, bool expected)
    {
        // Act
        var result = _smsService.IsValidPhoneNumber(phoneNumber);

        // Assert
        result.Should().Be(expected);
    }

    [Fact]
    public async Task SendSmsAsync_WithValidParameters_ReturnsTrue()
    {
        // Arrange
        var phoneNumber = "+1234567890";
        var message = "Test message";

        // Act
        var result = await _smsService.SendSmsAsync(phoneNumber, message);

        // Assert
        result.Should().BeTrue();
        _loggerMock.Verify(x => x.LogInfo(It.Is<string>(msg => msg.Contains("SMS sent successfully"))), Times.Once);
    }


    [Theory]
    [InlineData("", "message")]
    [InlineData("+1234567890", "")]
    [InlineData(null, "message")]
    [InlineData("+1234567890", null)]
    public async Task SendSmsAsync_WithMissingParameters_ReturnsFalse(string phoneNumber, string message)
    {
        // Act
        var result = await _smsService.SendSmsAsync(phoneNumber, message);

        // Assert
        result.Should().BeFalse();
        _loggerMock.Verify(x => x.LogWarning(It.Is<string>(msg => msg.Contains("missing required parameters"))), Times.Once);
    }
}

// LoggerTest.cs - Tests pour Logger
public class LoggerTest
{
    private readonly Logger _logger;

    public LoggerTest()
    {
        _logger = new Logger();
    }

    [Fact]
    public void LogInfo_WithMessage_AddsToLogCollection()
    {
        // Arrange
        var message = "Test info message";

        // Act
        _logger.LogInfo(message);

        // Assert
        var logs = _logger.GetLogs();
        logs.Should().HaveCount(1);
        logs[0].Should().Contain("[INFO]").And.Contain(message);
    }

    [Fact]
    public void LogError_WithMessage_AddsToLogCollection()
    {
        // Arrange
        var message = "Test error message";

        // Act
        _logger.LogError(message);

        // Assert
        var logs = _logger.GetLogs();
        logs.Should().HaveCount(1);
        logs[0].Should().Contain("[ERROR]").And.Contain(message);
    }

    [Fact]
    public void LogWarning_WithMessage_AddsToLogCollection()
    {
        // Arrange
        var message = "Test warning message";

        // Act
        _logger.LogWarning(message);

        // Assert
        var logs = _logger.GetLogs();
        logs.Should().HaveCount(1);
        logs[0].Should().Contain("[WARNING]").And.Contain(message);
    }

    [Fact]
    public void MultipleLogs_WithDifferentLevels_AddsAllToCollection()
    {
        // Arrange
        var infoMessage = "Info message";
        var errorMessage = "Error message";
        var warningMessage = "Warning message";

        // Act
        _logger.LogInfo(infoMessage);
        _logger.LogError(errorMessage);
        _logger.LogWarning(warningMessage);

        // Assert
        var logs = _logger.GetLogs();
        logs.Should().HaveCount(3);
        logs.Should().Contain(log => log.Contains("[INFO]") && log.Contains(infoMessage));
        logs.Should().Contain(log => log.Contains("[ERROR]") && log.Contains(errorMessage));
        logs.Should().Contain(log => log.Contains("[WARNING]") && log.Contains(warningMessage));
    }

    [Fact]
    public void ClearLogs_AfterLogging_EmptiesLogCollection()
    {
        // Arrange
        _logger.LogInfo("Test message");
        _logger.GetLogs().Should().HaveCount(1);

        // Act
        _logger.ClearLogs();

        // Assert
        _logger.GetLogs().Should().BeEmpty();
    }
}