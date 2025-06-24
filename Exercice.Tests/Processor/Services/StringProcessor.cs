using Processor.Contracts;
using System.Text;

namespace Processor.Services;

public class StringProcessor : IStringProcessor
{
    public string Reverse(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input ?? string.Empty;

        var chars = input.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    public bool IsPalindrome(string input)
    {
        if (string.IsNullOrEmpty(input))
            return true;

        // nettoyer la chaîne : supprimer espaces et ponctuation, convertir en minuscules
        var cleanedInput = new StringBuilder();
        foreach (char c in input)
        {
            if (char.IsLetterOrDigit(c))
                cleanedInput.Append(char.ToLower(c));
        }

        string cleaned = cleanedInput.ToString();
        if (string.IsNullOrEmpty(cleaned))
            return true;

        // vérifier si la chaîne nettoyée est égale à son inverse
        int left = 0;
        int right = cleaned.Length - 1;

        while (left < right)
        {
            if (cleaned[left] != cleaned[right])
                return false;
            left++;
            right--;
        }

        return true;
    }

    public int CountWords(string input)
    {
        if (string.IsNullOrEmpty(input))
            return 0;

        // diviser par les espaces et supprimer les entrées vides
        var words = input.Split(new char[] { ' ', '\t', '\n', '\r' },
                              StringSplitOptions.RemoveEmptyEntries);
        return words.Length;
    }

    public string Capitalize(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input ?? string.Empty;

        if (input.Length == 1)
            return char.ToUpper(input[0]).ToString();

        return char.ToUpper(input[0]) + input.Substring(1);
    }
}