using System.Text.RegularExpressions;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Helper;

/// <summary>
///     Works out how far each line should be indented from the brackets around it, whatever its indentation is now,
///     for languages like JavaScript where braces open on the line of their statement.
///     <list type="bullet">
///         <item>Inside (, [ or { a line is one level deeper than the line that opened it, however many brackets
///         that line opened: the body of foo(function () { is one level in. A line starting with a closing bracket
///         is at the level of the line that opened it.</item>
///         <item>In a switch, case and default are one level in and the lines under them two.</item>
///         <item>A line that goes on with the statement before it is one level deeper: one starting with . or an
///         operator (.then(...), + b, ? a : b), one after a line ending with an operator (=, +, &amp;&amp;, =&gt;...),
///         and the body of an if, for, while or else without braces.</item>
///         <item>In a /* */ comment that starts its line, lines starting with * go under its first *. Other lines
///         inside a comment move with the line it starts on, keeping their alignment.</item>
///         <item>Lines that start inside a literal (a multi-line template string) and blank lines are left
///         alone.</item>
///     </list>
/// </summary>
public sealed partial class BraceIndenter
{
    private const char LiteralFiller = '_';

    private static readonly string[] _leadingContinuations =
        [".", "?", ":", "&&", "||", "??", "+", "-", "=>"];

    private static readonly string[] _trailingContinuations =
        ["=", "+", "-", "*", "/", "%", "&&", "||", "??", "?", ":", "=>", "|", "&", "^"];

    private readonly int _indentSize;
    private readonly ICodeScanner _scanner;

    /// <param name="indentSize">Spaces per level.</param>
    /// <param name="scanner">Finds the comments and literals, whose brackets don't count.</param>
    public BraceIndenter(int indentSize, ICodeScanner scanner)
    {
        _indentSize = indentSize;
        _scanner = scanner;
    }

    /// <summary>
    ///     An open bracket: the level of the line that opened it, and whether it is a switch's body.
    /// </summary>
    private readonly record struct Frame(int Level, bool IsSwitch);

    /// <summary>
    ///     The indentation, in spaces, each line of the content should have, or null for a line to leave as it is.
    /// </summary>
    public int?[] ExpectedIndents(string content)
    {
        List<LineModel> lines = LineParser.GetLines(content);
        string code = CodeOnly(content);
        SpanStart[] startsInside = InsideSpans(content, lines);
        var result = new int?[lines.Count];
        var frames = new Stack<Frame>();
        string previousCode = string.Empty;
        bool previousLeftOpen = false;
        int commentShift = 0;
        int? starIndent = null;
        int lineStart = 0;

        for (int i = 0; i < lines.Count; i++)
        {
            string line = lines[i].Content;
            string lineCode = code.Substring(lineStart, line.Length).Trim();
            int current = IndentWidth(line);
            bool isBlank = string.IsNullOrWhiteSpace(line);
            int level = LevelOf(frames);

            if (startsInside[i] == SpanStart.Comment)
            {
                // The * of a /** ... */ comment goes under the first *; other text keeps its alignment
                result[i] = isBlank ? null
                    : line.TrimStart().StartsWith('*') && starIndent is { } star ? star
                    : Math.Max(0, current + commentShift);
            }
            else if (startsInside[i] == SpanStart.Code && !isBlank)
            {
                level = LevelOf(lineCode, frames, previousLeftOpen ? string.Empty : previousCode);
                result[i] = level * _indentSize;
                commentShift = level * _indentSize - current;
                starIndent = line.TrimStart().StartsWith("/*", StringComparison.Ordinal)
                    ? level * _indentSize + 1
                    : null;
            }

            // Brackets count even after a literal or comment that started on an earlier line
            bool leftOpen = Track(lineCode, frames, level);

            if (lineCode.Length > 0)
            {
                previousCode = lineCode;
                previousLeftOpen = leftOpen;
            }

            lineStart += line.Length + lines[i].LineEnding.ToText().Length;
        }

        return result;
    }

    private static int LevelOf(Stack<Frame> frames) => frames.Count == 0 ? 0 : frames.Peek().Level + 1;

    /// <param name="lineCode">The line's code, trimmed; empty for a comment line.</param>
    /// <param name="frames">The brackets open before the line.</param>
    /// <param name="previousCode">
    ///     The code of the line before, or empty when that line left a bracket open: then this line is already one
    ///     level in, and doesn't go on with it.
    /// </param>
    private static int LevelOf(string lineCode, Stack<Frame> frames, string previousCode)
    {
        if (lineCode.Length > 0 && lineCode[0] is ')' or ']' or '}')
        {
            return frames.Count == 0 ? 0 : frames.Peek().Level;
        }

        int level = LevelOf(frames);

        if (frames.Count > 0 && frames.Peek().IsSwitch && !CaseLabel().IsMatch(lineCode))
        {
            level++;
        }

        // A comment line (no code) takes the level of the code around it
        if (lineCode.Length > 0 && GoesOn(previousCode, lineCode))
        {
            level++;
        }

        return level;
    }

    /// <summary>
    ///     True when the line goes on with the statement of the line before it.
    /// </summary>
    private static bool GoesOn(string previousCode, string lineCode)
    {
        if (previousCode.Length == 0)
        {
            return false;
        }

        // After a , or ; a new item or statement starts, even with a sign: [1,\n-1]
        bool startsWithOperator = _leadingContinuations.Any(op => lineCode.StartsWith(op, StringComparison.Ordinal))
            && !lineCode.StartsWith("...", StringComparison.Ordinal)
            && !lineCode.StartsWith("++", StringComparison.Ordinal)
            && !lineCode.StartsWith("--", StringComparison.Ordinal)
            && previousCode[^1] is not (',' or ';');

        bool endsWithOperator = _trailingContinuations.Any(op => previousCode.EndsWith(op, StringComparison.Ordinal))
            && !previousCode.EndsWith("++", StringComparison.Ordinal)
            && !previousCode.EndsWith("--", StringComparison.Ordinal)
            && !CaseLabel().IsMatch(previousCode);

        return startsWithOperator || endsWithOperator || BracelessHeader().IsMatch(previousCode);
    }

    /// <summary>
    ///     Opens and closes the brackets of a line's code. Returns true if a bracket the line opened is still open at
    ///     its end: foo( is, foo(a) and }).then(x) are not.
    /// </summary>
    private static bool Track(string lineCode, Stack<Frame> frames, int level)
    {
        int lowest = frames.Count;

        for (int i = 0; i < lineCode.Length; i++)
        {
            char c = lineCode[i];

            if (c is '(' or '[' or '{')
            {
                bool isSwitch = c == '{' && SwitchHeader().IsMatch(lineCode[..i]);

                frames.Push(new Frame(level, isSwitch));
            }
            else if (c is ')' or ']' or '}' && frames.Count > 0)
            {
                frames.Pop();
                lowest = Math.Min(lowest, frames.Count);
            }
        }

        return frames.Count > lowest;
    }

    /// <summary>
    ///     The content with comments blanked out and literals filled, so only the code's brackets and operators are
    ///     left. Line ends stay where they are, so the lines still line up.
    /// </summary>
    private string CodeOnly(string content)
    {
        char[] code = content.ToCharArray();
        var spans = _scanner.FindComments(content)
            .Select(comment => (comment.Start, comment.End, Filler: ' '))
            .Concat(_scanner.FindLiterals(content)
                .Select(literal => (literal.Start, literal.End, Filler: LiteralFiller)));

        foreach (var (start, end, filler) in spans)
        {
            for (int i = start; i < end; i++)
            {
                if (code[i] is not ('\r' or '\n'))
                {
                    code[i] = filler;
                }
            }
        }

        return new string(code);
    }

    private enum SpanStart
    {
        Code,
        Comment,
        Literal,
    }

    /// <summary>
    ///     For each line, whether it starts in code, inside a /* */ comment or inside a literal.
    /// </summary>
    private SpanStart[] InsideSpans(string content, List<LineModel> lines)
    {
        var spans = _scanner.FindComments(content)
            .Where(comment => comment.IsBlock)
            .Select(comment => (comment.Start, comment.End, Kind: SpanStart.Comment))
            .Concat(_scanner.FindLiterals(content)
                .Select(literal => (literal.Start, literal.End, Kind: SpanStart.Literal)))
            .OrderBy(span => span.Start)
            .ToList();

        var result = new SpanStart[lines.Count];
        int next = 0;
        int lineStart = 0;

        for (int i = 0; i < lines.Count; i++)
        {
            while (next < spans.Count && spans[next].End <= lineStart)
            {
                next++;
            }

            result[i] = next < spans.Count && spans[next].Start < lineStart ? spans[next].Kind : SpanStart.Code;
            lineStart += lines[i].Content.Length + lines[i].LineEnding.ToText().Length;
        }

        return result;
    }

    private int IndentWidth(string line)
    {
        int width = 0;

        foreach (char c in line)
        {
            if (c == ' ')
            {
                width++;
            }
            else if (c == '\t')
            {
                width += _indentSize;
            }
            else
            {
                break;
            }
        }

        return width;
    }

    [GeneratedRegex(@"^(case\b.*|default\s*):")]
    private static partial Regex CaseLabel();

    [GeneratedRegex(@"\bswitch\s*\(.*\)\s*$")]
    private static partial Regex SwitchHeader();

    // if (x), } else if (x), for (...), while (...) or else, with nothing after: the next line is the body
    [GeneratedRegex(@"^(\}\s*)?((else\s+)?if|for|while)\b.*\)$|^(\}\s*)?else$")]
    private static partial Regex BracelessHeader();
}
