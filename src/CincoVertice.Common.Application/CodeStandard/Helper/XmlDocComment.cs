using System.Text.RegularExpressions;

namespace CincoVertice.Common.Application.CodeStandard.Helper;

/// <summary>
///     XML documentation comment (///) patterns shared by the empty XML comment checker and fixer,
///     so both agree on what counts as empty.
/// </summary>
public static class XmlDocComment
{
    // An open/close pair with nothing between: <summary></summary>, <param name="x"></param>
    private static readonly Regex _emptyTagPair =
        new(@" ?<([\w:]+)(?:\s[^>]*)?></\1>", RegexOptions.Compiled);

    // /// with nothing after
    private static readonly Regex _blankLine =
        new(@"^\s*///\s*$", RegexOptions.Compiled);

    // /// with only an opening or closing tag: <summary>, </summary>, <param name="x">.
    // Self-closing tags such as <inheritdoc/> are content.
    private static readonly Regex _tagOnlyLine =
        new(@"^\s*///\s*</?[\w:]+(?:\s[^>]*)?(?<!/)>\s*$", RegexOptions.Compiled);

    /// <summary>
    ///     True for a /// line. Four or more slashes (e.g. separator lines) are not documentation.
    /// </summary>
    public static bool IsDocLine(string text)
    {
        var trimmed = text.AsSpan().TrimStart();

        return trimmed.StartsWith("///") && !trimmed.StartsWith("////");
    }

    public static bool ContainsEmptyTag(string text) => _emptyTagPair.IsMatch(text);

    /// <summary>
    ///     Removes every empty tag pair and the space before it.
    /// </summary>
    public static string RemoveEmptyTags(string text) => _emptyTagPair.Replace(text, string.Empty);

    /// <summary>
    ///     True for a doc line with no text: blank, or only an opening or closing tag.
    /// </summary>
    public static bool IsWithoutText(string text) => _blankLine.IsMatch(text) || _tagOnlyLine.IsMatch(text);
}
