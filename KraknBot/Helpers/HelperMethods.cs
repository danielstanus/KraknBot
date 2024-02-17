using System.Collections.Generic;

namespace KraknBot.Helpers;

public class HelperMethods
{
    public static string ConvertNonEnglishCharactersToEnglish(string input)
    {
        var replacements = new Dictionary<string, string>
        {
            {"ä", "a"}, {"ö", "o"}, {"ü", "u"}, // Add more mappings here
            {"ç", "c"}, {"ş", "s"}, {"ğ", "g"},
            {"Ä", "A"}, {"Ö", "O"}, {"Ü", "U"},
            {"Ç", "C"}, {"Ş", "S"}, {"Ğ", "G"}
            // Extend with other non-English characters as required
        };

        foreach (var pair in replacements)
        {
            input = input.Replace(pair.Key, pair.Value);
        }

        return input;
    }
}