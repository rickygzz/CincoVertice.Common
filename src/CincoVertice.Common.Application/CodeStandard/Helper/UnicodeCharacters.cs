using System.Globalization;

namespace CincoVertice.Common.Application.CodeStandard.Helper;

/// <summary>
///     Unicode characters that look like nothing or like a normal space, often pasted from AI chats, web pages
///     or Word documents. Shared by the invisible-character checker and fixer, so both agree on what is wrong.
/// </summary>
public static class UnicodeCharacters
{
    private static readonly Dictionary<char, string> _names = new()
    {
        ['\u00A0'] = "no-break space",
        ['\u00AD'] = "soft hyphen",
        ['\u2009'] = "thin space",
        ['\u200B'] = "zero width space",
        ['\u200C'] = "zero width non-joiner",
        ['\u200D'] = "zero width joiner",
        ['\u200E'] = "left-to-right mark",
        ['\u200F'] = "right-to-left mark",
        ['\u202F'] = "narrow no-break space",
        ['\u2060'] = "word joiner",
        ['\u3000'] = "ideographic space",
        ['\uFEFF'] = "byte order mark",
    };

    /// <summary>
    ///     Invisible characters: zero width characters, soft hyphen, byte order mark and bidirectional controls.
    ///     Bidirectional controls can make code display differently from how it compiles ("Trojan Source").
    /// </summary>
    public static bool IsInvisible(char c)
    {
        return c is '\u00AD' or '\u180E' or '\uFEFF'
            or (>= '\u200B' and <= '\u200F')
            or (>= '\u202A' and <= '\u202E')
            or (>= '\u2060' and <= '\u2064')
            or (>= '\u2066' and <= '\u2069');
    }

    /// <summary>
    ///     Space characters other than the normal space, such as the no-break space (U+00A0).
    /// </summary>
    public static bool IsUnicodeSpace(char c)
    {
        return c != ' ' && char.GetUnicodeCategory(c) == UnicodeCategory.SpaceSeparator;
    }

    /// <summary>
    ///     Code point and name, e.g. "U+200B (zero width space)".
    /// </summary>
    public static string Describe(char c)
    {
        string codePoint = $"U+{(int)c:X4}";

        return _names.TryGetValue(c, out string? name) ? $"{codePoint} ({name})" : codePoint;
    }
}
