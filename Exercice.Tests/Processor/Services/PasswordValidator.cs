using Processor.Contracts;
using Processor.Model;

namespace Processor.Services;

public class PasswordValidator : IPasswordValidator
{
    private const int MinLength = 8;
    private const int MaxLength = 50;
    private readonly string SpecialCharacters = "!@#$%^&*()_+-=[]{}|;':\",./<>?";

    public ValidationResult Validate(string password)
    {
        var result = new ValidationResult();

        if (string.IsNullOrEmpty(password))
        {
            result.AddError("Le mot de passe ne peut pas être vide");
            return result;
        }

        if (password.Contains(' '))
            result.AddError("Le mot de passe ne peut pas contenir d'espaces");

        if (password.Length < MinLength)
            result.AddError($"Le mot de passe doit contenir au moins {MinLength} caractères");

        if (password.Length > MaxLength)
            result.AddError($"Le mot de passe ne peut pas dépasser {MaxLength} caractères");

        if (!password.Any(char.IsUpper))
            result.AddError("Le mot de passe doit contenir au moins une majuscule");

        if (!password.Any(char.IsLower))
            result.AddError("Le mot de passe doit contenir au moins une minuscule");

        if (!password.Any(char.IsDigit))
            result.AddError("Le mot de passe doit contenir au moins un chiffre");

        if (!password.Any(c => SpecialCharacters.Contains(c)))
            result.AddError("Le mot de passe doit contenir au moins un caractère spécial (!@#$%^&*()_+-=[]{}|;':\",./<>?)");

        return result;
    }
}