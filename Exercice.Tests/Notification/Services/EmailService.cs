using System.Text.RegularExpressions;
using Notification.Contracts;

namespace Notification.Services;

public class EmailService : IEmailService
{
    private readonly ILogger _logger;

    public EmailService(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(to) || string.IsNullOrWhiteSpace(subject))
            {
                _logger?.LogWarning("Email sending failed: missing required parameters");
                return false;
            }

            // simulation d'envoi d'email
            await Task.Delay(100); // simule un appel réseau

            _logger?.LogInfo($"Email sent successfully to {to} with subject '{subject}'");
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError($"Exception while sending email: {ex.Message}");
            throw;
        }
    }

    public bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        const string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
        return Regex.IsMatch(email, emailPattern);
    }

    public void SendWelcomeEmail(string email, string name)
    {
        try
        {
            if (!IsValidEmail(email))
            {
                _logger?.LogWarning($"Invalid email format for welcome email: {email}");
                return;
            }

            var subject = "Welcome to our platform!";
            var body = $"Dear {name},\n\nWelcome to our platform! We're excited to have you on board.";

            var task = SendEmailAsync(email, subject, body);
            task.Wait();

            _logger?.LogInfo($"Welcome email process completed for {name} ({email})");
        }
        catch (Exception ex)
        {
            _logger?.LogError($"Error sending welcome email: {ex.Message}");
            throw;
        }
    }
}