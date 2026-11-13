using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Helper;

/// <summary>
///     Finds the comments and literals of JavaScript: '...' and "..." strings, `...` template strings (which can span
///     lines, with ${...} holes of code that may hold strings of their own) and /.../ regular expressions.
///     <para>
///         A / starts a regular expression where a value is expected: at the start, after an operator or (, [, {, ,
///         and after keywords such as return. After a name, a number, ), ] or a literal it divides. A / that would
///         start a regular expression but isn't closed on its line is taken as a division.
///     </para>
/// </summary>
public sealed class JavaScriptCodeScanner : ICodeScanner
{
    // After these words a value is expected, so a / starts a regular expression: return /x/.test(s)
    private static readonly HashSet<string> _keywordsBeforeValue =
    [
        "return", "typeof", "instanceof", "in", "of", "new", "delete", "void", "throw", "case", "do", "else",
        "yield", "await",
    ];

    public static JavaScriptCodeScanner Instance { get; } = new();

    public IEnumerable<CommentSpanModel> FindComments(string content)
    {
        return Scan(content)
            .Where(span => span.Kind != SpanKind.Literal)
            .Select(span => new CommentSpanModel(span.Start, span.End, span.Kind == SpanKind.BlockComment));
    }

    public IEnumerable<(int Start, int End)> FindLiterals(string content)
    {
        return Scan(content)
            .Where(span => span.Kind == SpanKind.Literal)
            .Select(span => (span.Start, span.End));
    }

    private enum SpanKind
    {
        LineComment,
        BlockComment,
        Literal,
    }

    /// <summary>
    ///     What came last before a /, to tell a regular expression from a division.
    /// </summary>
    private enum Previous
    {
        Nothing,
        Word,
        Value,
        Operator,
    }

    private static IEnumerable<(int Start, int End, SpanKind Kind)> Scan(string content)
    {
        Previous previous = Previous.Nothing;
        string previousWord = string.Empty;
        int i = 0;

        while (i < content.Length)
        {
            char c = content[i];
            char next = i + 1 < content.Length ? content[i + 1] : '\0';

            if (char.IsWhiteSpace(c))
            {
                i++;
            }
            else if (c == '/' && next == '/')
            {
                int end = IndexOfLineEnd(content, i);

                yield return (i, end, SpanKind.LineComment);
                i = end;
            }
            else if (c == '/' && next == '*')
            {
                int end = SkipBlockComment(content, i);

                yield return (i, end, SpanKind.BlockComment);
                i = end;
            }
            else if (c is '\'' or '"' or '`' || (c == '/' && StartsRegex(previous, previousWord)))
            {
                int end = SkipLiteral(content, i);

                // A / whose regular expression isn't closed on its line divides
                if (end < 0)
                {
                    previous = Previous.Operator;
                    i++;

                    continue;
                }

                yield return (i, end, SpanKind.Literal);
                previous = Previous.Value;
                i = end;
            }
            else if (IsWordChar(c))
            {
                int start = i;

                while (i < content.Length && IsWordChar(content[i]))
                {
                    i++;
                }

                previous = Previous.Word;
                previousWord = content[start..i];
            }
            else
            {
                previous = c is ')' or ']' or '}' ? Previous.Value : Previous.Operator;
                i++;
            }
        }
    }

    private static bool StartsRegex(Previous previous, string previousWord) => previous switch
    {
        Previous.Nothing or Previous.Operator => true,
        Previous.Word => _keywordsBeforeValue.Contains(previousWord),
        _ => false,
    };

    private static bool IsWordChar(char c) => char.IsLetterOrDigit(c) || c is '_' or '$';

    private static int IndexOfLineEnd(string content, int start)
    {
        int end = content.IndexOfAny(['\r', '\n'], start);

        return end == -1 ? content.Length : end;
    }

    private static int SkipBlockComment(string content, int start)
    {
        int close = content.IndexOf("*/", start + 2, StringComparison.Ordinal);

        return close == -1 ? content.Length : close + 2;
    }

    /// <summary>
    ///     End (exclusive) of the literal starting at start, or -1 for a regular expression not closed on its line.
    /// </summary>
    private static int SkipLiteral(string content, int start) => content[start] switch
    {
        '`' => SkipTemplate(content, start + 1),
        '/' => SkipRegex(content, start + 1),
        _ => SkipQuoted(content, start + 1, content[start]),
    };

    /// <summary>
    ///     '...' or "...": backslash escapes; ends at the closing quote or the end of the line.
    /// </summary>
    private static int SkipQuoted(string content, int i, char quote)
    {
        while (i < content.Length && content[i] is not ('\r' or '\n'))
        {
            if (content[i] == '\\')
            {
                i += 2;
            }
            else if (content[i] == quote)
            {
                return i + 1;
            }
            else
            {
                i++;
            }
        }

        return Math.Min(i, content.Length);
    }

    /// <summary>
    ///     `...`: backslash escapes, may span lines, and ${ starts a hole of code.
    /// </summary>
    private static int SkipTemplate(string content, int i)
    {
        while (i < content.Length)
        {
            char c = content[i];

            if (c == '\\')
            {
                i += 2;
            }
            else if (c == '`')
            {
                return i + 1;
            }
            else if (c == '$' && i + 1 < content.Length && content[i + 1] == '{')
            {
                i = SkipHole(content, i + 2);
            }
            else
            {
                i++;
            }
        }

        return content.Length;
    }

    /// <summary>
    ///     End (after the closing }) of a ${...} hole starting at i, skipping the literals, comments and nested
    ///     braces in it.
    /// </summary>
    private static int SkipHole(string content, int i)
    {
        int depth = 0;

        while (i < content.Length)
        {
            char c = content[i];
            char next = i + 1 < content.Length ? content[i + 1] : '\0';

            if (c is '\'' or '"' or '`')
            {
                i = SkipLiteral(content, i);

                continue;
            }

            if (c == '/' && next == '/')
            {
                i = IndexOfLineEnd(content, i);

                continue;
            }

            if (c == '/' && next == '*')
            {
                i = SkipBlockComment(content, i);

                continue;
            }

            if (c == '{')
            {
                depth++;
            }
            else if (c == '}')
            {
                if (depth == 0)
                {
                    return i + 1;
                }

                depth--;
            }

            i++;
        }

        return content.Length;
    }

    /// <summary>
    ///     /.../flags: backslash escapes, and a / inside [...] doesn't end it. Must end on its line.
    /// </summary>
    private static int SkipRegex(string content, int i)
    {
        bool inClass = false;

        while (i < content.Length && content[i] is not ('\r' or '\n'))
        {
            char c = content[i];

            if (c == '\\')
            {
                i += 2;

                continue;
            }

            if (c == '[')
            {
                inClass = true;
            }
            else if (c == ']')
            {
                inClass = false;
            }
            else if (c == '/' && !inClass)
            {
                i++;

                while (i < content.Length && char.IsAsciiLetter(content[i]))
                {
                    i++;
                }

                return i;
            }

            i++;
        }

        return -1;
    }
}
