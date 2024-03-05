using System.Globalization;

namespace WebApp.Utility;

public static class StringExtensions
{
    public static string FormatDateToFrenchDate(this string date, string format)
    {
        CultureInfo culture = new("fr-FR", true);

        return DateOnly.Parse(date).ToString(format, culture).Capitalize();
    }

    /// <summary>
    /// Capitalized all words in string
    /// 
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static string Capitalize(this string input)
    {
        string[] words = input.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < words.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(words[i]))
            {
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
            }
        }

        string capitalizedString = string.Join(" ", words);

        return capitalizedString;
    }
}
