using System.Text.RegularExpressions;
using Notification.Contracts;

namespace Notification.Services;

public class SmsService : ISmsService
{
    private readonly ILogger _logger;

    public SmsService(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<bool> SendSmsAsync(string phoneNumber, string message)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(phoneNumber) || string.IsNullOrWhiteSpace(message))
            {
                _logger?.LogWarning("SMS sending failed: missing required parameters");
                return false;
            }

            // simulation d'envoi de SMS
            await Task.Delay(150); // simule un appel API

            _logger?.LogInfo($"SMS sent successfully to {phoneNumber}");
            return true;
        }
        catch (Exception ex)
        {
            _logger?.LogError($"Exception while sending SMS: {ex.Message}");
            throw;
        }
    }

    public bool IsValidPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        // pattern pour les numéro international (+33, +1, etc.) ou national
        const string phonePattern = @"^(\+\d{1,3}[- ]?)?\d{10,14}$";
        return Regex.IsMatch(phoneNumber.Replace(" ", "").Replace("-", ""), phonePattern);
    }
}
