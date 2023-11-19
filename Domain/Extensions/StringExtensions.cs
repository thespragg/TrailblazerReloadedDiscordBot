using System.Globalization;

namespace Domain.Extensions;

public static class StringExtensions
{
    public static string ToTitleCase(this string input)
    {
        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
    }
}