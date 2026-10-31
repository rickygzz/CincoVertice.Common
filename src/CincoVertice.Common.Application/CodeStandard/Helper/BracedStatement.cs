using System.Text.RegularExpressions;

namespace CincoVertice.Common.Application.CodeStandard.Helper;

/// <summary>
///     Patterns for the statements whose bodies need braces (if, else if, else, for, foreach, while), shared by the
///     missing braces checker and fixer, so both agree on which statements need braces. Both match the start of a
///     trimmed line, including "} else".
/// </summary>
public static partial class BracedStatement
{
    /// <summary>
    ///     Matches "if (", "else if (", "for (", "foreach (" or "while (", optionally after "}", ending after the
    ///     opening parenthesis. The <c>keyword</c> group holds the keyword, so callers can tell an if, which may be
    ///     followed by an else, from a loop.
    /// </summary>
    public static Match MatchHeader(string trimmed) => HeaderRegex().Match(trimmed);

    /// <summary>
    ///     Matches "else" or "} else" as a whole word. Also matches "else if", so check <see cref="MatchHeader"/>
    ///     first.
    /// </summary>
    public static Match MatchElse(string trimmed) => ElseRegex().Match(trimmed);

    [GeneratedRegex(@"^(?:}\s*)?(?:else\s+)?(?<keyword>if|for|foreach|while)\s*\(")]
    private static partial Regex HeaderRegex();

    [GeneratedRegex(@"^(?:}\s*)?else\b")]
    private static partial Regex ElseRegex();
}
