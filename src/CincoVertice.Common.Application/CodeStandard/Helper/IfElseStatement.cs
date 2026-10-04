using System.Text.RegularExpressions;

namespace CincoVertice.Common.Application.CodeStandard.Helper;

/// <summary>
///     if / else if / else line patterns shared by the missing braces checker and fixer, so both agree on which
///     statements need braces. Both match the start of a trimmed line, including "} else".
/// </summary>
public static partial class IfElseStatement
{
    /// <summary>
    ///     Matches "if (", "else if (" or "} else if (", ending after the opening parenthesis.
    /// </summary>
    public static Match MatchIf(string trimmed) => IfOrElseIfRegex().Match(trimmed);

    /// <summary>
    ///     Matches "else" or "} else" followed by whitespace or the line end. Also matches "else if", so check
    ///     <see cref="MatchIf"/> first.
    /// </summary>
    public static Match MatchElse(string trimmed) => ElseRegex().Match(trimmed);

    [GeneratedRegex(@"^(?:}\s*)?(?:else\s+)?if\s*\(")]
    private static partial Regex IfOrElseIfRegex();

    [GeneratedRegex(@"^(?:}\s*)?else(?:\s|$)")]
    private static partial Regex ElseRegex();
}
