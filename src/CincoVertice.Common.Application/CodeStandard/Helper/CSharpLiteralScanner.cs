using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Helper;

/// <summary>
///     Finds where C# string and char literals end, so code and comments can be told apart from text inside
///     literals. Handles regular, verbatim (@), interpolated ($) and raw (""") strings, and char literals.
///     <para>Limitation: quotes inside an interpolation hole, as in $"{a["x"]}", end the string early. The
///     scanner resynchronizes at the next quote.</para>
/// </summary>
public static class CSharpLiteralScanner
{
    /// <summary>
    ///     The // and /* */ comments in C# code, in order, ignoring comment delimiters inside literals.
    ///     <para>For a whole file, literals spanning several lines (verbatim and raw strings) are followed.
    ///     An unclosed /* comment runs to the end of the content.</para>
    /// </summary>
    /// <param name="content">C# code: a whole file or a single line.</param>
    public static IEnumerable<CommentSpanModel> FindComments(string content)
    {
        int i = 0;

        while (i < content.Length)
        {
            char c = content[i];
            char next = i + 1 < content.Length ? content[i + 1] : '\0';

            if (c == '/' && next == '/')
            {
                int end = IndexOfLineEnd(content, i);

                yield return new CommentSpanModel(i, end, IsBlock: false);
                i = end;
            }
            else if (c == '/' && next == '*')
            {
                int close = content.IndexOf("*/", i + 2, StringComparison.Ordinal);
                int end = close == -1 ? content.Length : close + 2;

                yield return new CommentSpanModel(i, end, IsBlock: true);
                i = end;
            }
            else if (c is '"' or '\'' or '@' or '$')
            {
                i = SkipLiteral(content, i);
            }
            else
            {
                i++;
            }
        }
    }

    /// <summary>
    ///     Start of a // comment in a single line of code, ignoring // inside literals and /* */ comments.
    /// </summary>
    /// <param name="text">One line of code.</param>
    /// <returns>Index of the //, or -1 if the line has no // comment.</returns>
    public static int FindLineComment(string text)
    {
        foreach (var comment in FindComments(text))
        {
            if (!comment.IsBlock)
            {
                return comment.Start;
            }
        }

        return -1;
    }

    private static int IndexOfLineEnd(string content, int start)
    {
        int end = content.IndexOfAny(['\r', '\n'], start);

        return end == -1 ? content.Length : end;
    }

    /// <summary>
    ///     End (exclusive) of the string or char literal starting at start. When start is not the beginning of a
    ///     literal (e.g. an @identifier), returns start + 1, so the character is copied as normal code.
    /// </summary>
    public static int SkipLiteral(string content, int start)
    {
        if (content[start] == '\'')
        {
            return SkipQuoted(content, start + 1, '\'');
        }

        // Prefixes: $ (interpolated), @ (verbatim), or both, in any order
        int quote = start;
        bool isVerbatim = false;

        while (quote < content.Length && content[quote] is '$' or '@')
        {
            isVerbatim |= content[quote] == '@';
            quote++;
        }

        if (quote >= content.Length || content[quote] != '"')
        {
            return start + 1;
        }

        int quoteCount = 0;

        while (quote + quoteCount < content.Length && content[quote + quoteCount] == '"')
        {
            quoteCount++;
        }

        // Raw string: three or more quotes (cannot be verbatim), closed by the same number of quotes
        if (quoteCount >= 3 && !isVerbatim)
        {
            string delimiter = new('"', quoteCount);
            int close = content.IndexOf(delimiter, quote + quoteCount, StringComparison.Ordinal);

            return close == -1 ? content.Length : close + quoteCount;
        }

        return isVerbatim ? SkipVerbatim(content, quote + 1) : SkipQuoted(content, quote + 1, '"');
    }

    /// <summary>
    ///     Regular string or char literal: backslash escapes; ends at the closing delimiter or the end of the line.
    /// </summary>
    private static int SkipQuoted(string content, int i, char delimiter)
    {
        while (i < content.Length && content[i] is not ('\r' or '\n'))
        {
            if (content[i] == '\\')
            {
                i += 2;
            }
            else if (content[i] == delimiter)
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
    ///     Verbatim string: "" is an escaped quote; may span several lines.
    /// </summary>
    private static int SkipVerbatim(string content, int i)
    {
        while (i < content.Length)
        {
            if (content[i] == '"')
            {
                if (i + 1 < content.Length && content[i + 1] == '"')
                {
                    i += 2;

                    continue;
                }

                return i + 1;
            }

            i++;
        }

        return content.Length;
    }
}
