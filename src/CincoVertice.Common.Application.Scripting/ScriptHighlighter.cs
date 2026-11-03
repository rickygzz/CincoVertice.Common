using CincoVertice.Common.Utils.Highlighting;

namespace CincoVertice.Common.Application.Scripting;

/// <summary>
///     Finds what to color in a script. Unlike <see cref="ScriptLexer"/> it never fails, since a script being typed is
///     usually broken: an unclosed text colors up to the end of its line, and what it doesn't know is left as it is.
/// </summary>
public sealed class ScriptHighlighter : ICodeHighlighter
{
    private const string MultiLineQuotes = "\"\"\"";

    public IReadOnlyList<CodeSpan> Find(string source)
    {
        List<CodeSpan> spans = [];
        int index = 0;

        while (index < source.Length)
        {
            char current = source[index];
            int start = index;

            if (current == '#')
            {
                index = EndOfLine(source, index);
                spans.Add(new CodeSpan(start, index - start, CodeSpanKind.Comment));
            }
            else if (string.CompareOrdinal(source, index, MultiLineQuotes, 0, MultiLineQuotes.Length) == 0)
            {
                index = EndOfMultiLineText(source, index);
                spans.Add(new CodeSpan(start, index - start, CodeSpanKind.Text));
            }
            else if (current == '"')
            {
                index = EndOfText(source, index);
                spans.Add(new CodeSpan(start, index - start, CodeSpanKind.Text));
            }
            else if (char.IsAsciiDigit(current) || (current == '.' && char.IsAsciiDigit(Peek(source, index + 1))))
            {
                index = EndOfNumber(source, index);
                spans.Add(new CodeSpan(start, index - start, CodeSpanKind.Number));
            }
            else if (char.IsAsciiLetter(current) || current == '_')
            {
                index = EndOfName(source, index);
                AddName(spans, source, start, index);
            }
            else
            {
                index++;
            }
        }

        return spans;
    }

    private static void AddName(List<CodeSpan> spans, string source, int start, int end)
    {
        string name = source[start..end];

        if (ScriptLexer.IsKeyword(name))
        {
            spans.Add(new CodeSpan(start, end - start, CodeSpanKind.Keyword));
        }
        else if (name.Contains('.') || Peek(source, end) == '(')
        {
            spans.Add(new CodeSpan(start, end - start, CodeSpanKind.Function));
        }
    }

    private static char Peek(string source, int index) => index < source.Length ? source[index] : '\0';

    private static int EndOfLine(string source, int index)
    {
        int end = source.IndexOf('\n', index);

        return end < 0 ? source.Length : end;
    }

    private static int EndOfText(string source, int index)
    {
        for (int i = index + 1; i < source.Length && source[i] != '\n'; i++)
        {
            if (source[i] == '\\')
            {
                i++;
            }
            else if (source[i] == '"')
            {
                return i + 1;
            }
        }

        return EndOfLine(source, index);
    }

    /// <summary>Past the """ that starts a later line, or the end of the script when it never comes.</summary>
    private static int EndOfMultiLineText(string source, int index)
    {
        int lineEnd = EndOfLine(source, index);

        while (lineEnd < source.Length)
        {
            int lineStart = lineEnd + 1;
            int quotes = lineStart;

            while (Peek(source, quotes) is ' ' or '\t')
            {
                quotes++;
            }

            if (string.CompareOrdinal(source, quotes, MultiLineQuotes, 0, MultiLineQuotes.Length) == 0)
            {
                return quotes + MultiLineQuotes.Length;
            }

            lineEnd = EndOfLine(source, lineStart);
        }

        return source.Length;
    }

    private static int EndOfNumber(string source, int index)
    {
        while (char.IsAsciiDigit(Peek(source, index)))
        {
            index++;
        }

        if (Peek(source, index) == '.' && char.IsAsciiDigit(Peek(source, index + 1)))
        {
            index++;

            while (char.IsAsciiDigit(Peek(source, index)))
            {
                index++;
            }
        }

        return index;
    }

    private static int EndOfName(string source, int index)
    {
        while (char.IsAsciiLetterOrDigit(Peek(source, index))
            || Peek(source, index) == '_'
            || (Peek(source, index) == '.'
                && (char.IsAsciiLetter(Peek(source, index + 1)) || Peek(source, index + 1) == '_')))
        {
            index++;
        }

        return index;
    }
}
