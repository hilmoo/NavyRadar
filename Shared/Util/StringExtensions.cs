using System.Text.RegularExpressions;

namespace Shared.Util;

public static partial class StringExtensions
{
    public static string ToSpacedString(this Enum value)
    {
        var enumName = value.ToString();
        return ToSpacedStringRegex().Replace(enumName, " $1");
    }

    [GeneratedRegex("(\\B[A-Z])")]
    private static partial Regex ToSpacedStringRegex();
}