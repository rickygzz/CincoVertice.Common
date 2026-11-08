using System.Text;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Fixers.CSharp;

/// <summary>
///     Fixes CH0031 (see MissingBracesChecker).
///     <para>A body without braces is the whole next statement, which may itself be an if with an else chain, a
///     loop or a block. Its end is found first, then it is wrapped in braces and fixed the same way, so nested
///     bodies get braces too.</para>
/// </summary>
public sealed class MissingBracesFixer : IStringFixer
{
    /// <summary>
    ///     Adds braces to <c>if</c>, <c>else if</c>, <c>else</c>, <c>for</c>, <c>foreach</c> and <c>while</c>
    ///     bodies that are missing them, with each brace on its own line.
    /// </summary>
    /// <param name="content">The file content to fix.</param>
    /// <returns>The content with braces added where missing.</returns>
    public string Fix(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        var lines = LineParser.GetLines(content);
        var sb = new StringBuilder(content.Length);

        FixLines(lines, sb);

        return sb.ToString();
    }

    private static void FixLines(List<LineModel> lines, StringBuilder sb)
    {
        int i = 0;

        while (i < lines.Count)
        {
            string trimmed = lines[i].TrimmedContent;

            if (BracedStatement.MatchHeader(trimmed).Success)
            {
                i = ProcessHeader(lines, i, sb);
            }
            else if (BracedStatement.MatchElse(trimmed).Success)
            {
                i = ProcessBareElse(lines, i, sb);
            }
            else
            {
                AppendLine(sb, lines[i]);
                i++;
            }
        }
    }

    private static int ProcessHeader(List<LineModel> lines, int i, StringBuilder sb)
    {
        string indent = GetIndent(lines[i].Content);
        string le = LineEndingOf(lines[i]);

        var (endLine, endCol) = FindConditionEnd(lines, i, 0);

        if (endLine < 0)
        {
            AppendLine(sb, lines[i]);
            return i + 1;
        }

        string lastContent = lines[endLine].Content;
        string afterParen = lastContent[(endCol + 1)..].Trim();

        // A ; body is an empty statement, or the end of a do-while: "} while (x);"
        if (afterParen.StartsWith("{") || afterParen.StartsWith(";"))
        {
            for (int j = i; j <= endLine; j++)
            {
                AppendLine(sb, lines[j]);
            }

            return endLine + 1;
        }

        for (int j = i; j < endLine; j++)
        {
            AppendLine(sb, lines[j]);
        }

        if (!string.IsNullOrWhiteSpace(afterParen) && !afterParen.StartsWith("//"))
        {
            // Single-line: if (cond) body;
            sb.Append(lastContent[..(endCol + 1)]).Append(le);
            MoveBodyToNextLine(lines, endLine, afterParen, indent);
        }
        else
        {
            AppendLine(sb, lines[endLine]);
        }

        return ProcessBodyOnNextLine(lines, endLine + 1, indent, le, sb);
    }

    private static int ProcessBareElse(List<LineModel> lines, int i, StringBuilder sb)
    {
        string indent = GetIndent(lines[i].Content);
        string le = LineEndingOf(lines[i]);
        string content = lines[i].Content;

        int elseEnd = content.IndexOf("else", StringComparison.Ordinal) + 4;
        string afterElse = content[elseEnd..].Trim();

        if (afterElse.StartsWith("{"))
        {
            AppendLine(sb, lines[i]);
            return i + 1;
        }

        if (!string.IsNullOrWhiteSpace(afterElse) && !afterElse.StartsWith("//"))
        {
            // Single-line: else body; (or } else body;)
            sb.Append(content[..elseEnd]).Append(le);
            MoveBodyToNextLine(lines, i, afterElse, indent);
        }
        else
        {
            AppendLine(sb, lines[i]);
        }

        return ProcessBodyOnNextLine(lines, i + 1, indent, le, sb);
    }

    /// <summary>
    ///     Moves a body written on its header line to new lines after <paramref name="lineIndex"/>: the body's
    ///     first statement, one level deeper, then the code after it (e.g. "else B();" in "if (x) A(); else B();"),
    ///     which is fixed next. A body that is itself a statement with a body (if (a) if (b) x; else y;) keeps the
    ///     rest of the line, because a following else belongs to it.
    /// </summary>
    private static void MoveBodyToNextLine(List<LineModel> lines, int lineIndex, string body, string indent)
    {
        int end = BracedStatement.MatchHeader(body).Success ? -1 : FindStatementEnd(body);
        string rest = end < 0 ? string.Empty : body[(end + 1)..].Trim();
        var lineEnding = lines[lineIndex].LineEnding;

        if (rest.Length > 0)
        {
            body = body[..(end + 1)];
            lines.Insert(lineIndex + 1, NewLine(indent + rest, lineEnding));
        }

        lines.Insert(lineIndex + 1, NewLine(indent + "    " + body, lineEnding));
    }

    private static int ProcessBodyOnNextLine(
        List<LineModel> lines, int i, string indent, string le, StringBuilder sb)
    {
        while (i < lines.Count && string.IsNullOrWhiteSpace(lines[i].Content))
        {
            AppendLine(sb, lines[i]);
            i++;
        }

        if (i >= lines.Count)
        {
            return i;
        }

        if (lines[i].TrimmedContent.StartsWith("{"))
        {
            AppendLine(sb, lines[i]);
            return i + 1;
        }

        int start = NextCodeLine(lines, i);
        int bodyEnd = start < 0 ? i : Math.Max(i, StatementEnd(lines, start, 0).Line);

        sb.Append(indent).Append("{").Append(le);
        FixLines(lines.GetRange(i, bodyEnd - i + 1), sb);

        if (sb[^1] != '\n')
        {
            sb.Append(le);
        }

        sb.Append(indent).Append("}").Append(LineEndingOf(lines[bodyEnd]));

        return bodyEnd + 1;
    }

    /// <summary>
    ///     Finds the end of the statement starting at (<paramref name="line"/>, <paramref name="col"/>): a block,
    ///     an if with its else chain, a loop with its body, or a plain statement ending in ; (or in the block of a
    ///     switch, try...). Returns the position of its last character.
    /// </summary>
    private static (int Line, int Col) StatementEnd(List<LineModel> lines, int line, int col)
    {
        string content = lines[line].Content;
        int start = col;

        while (start < content.Length && char.IsWhiteSpace(content[start]))
        {
            start++;
        }

        string code = content[start..];

        if (code.StartsWith("{"))
        {
            return OrEndOfLine(BlockEnd(lines, line, start), lines, line);
        }

        var header = BracedStatement.MatchHeader(code);

        if (header.Success)
        {
            var condition = FindConditionEnd(lines, line, start);

            if (condition.Line < 0)
            {
                return EndOfLine(lines, line);
            }

            var body = BodyEnd(lines, condition.Line, condition.Col + 1);

            return header.Groups["keyword"].Value == "if" ? ElseChainEnd(lines, body) : body;
        }

        return OrEndOfLine(PlainStatementEnd(lines, line, start), lines, line);
    }

    /// <summary>
    ///     Finds the end of the body that follows a header ending before <paramref name="col"/>, on the same line
    ///     or on the next code line.
    /// </summary>
    private static (int Line, int Col) BodyEnd(List<LineModel> lines, int line, int col)
    {
        string after = lines[line].Content[col..].TrimStart();

        if (after.Length == 0 || after.StartsWith("//"))
        {
            int next = NextCodeLine(lines, line + 1);

            return next < 0 ? EndOfLine(lines, line) : StatementEnd(lines, next, 0);
        }

        return StatementEnd(lines, line, col);
    }

    /// <summary>
    ///     Extends the end of an if body over the else or else if that follows it, on the same line or on the next
    ///     code line. A "} else" line does not count: its } closes an enclosing block, so the else is not ours.
    /// </summary>
    private static (int Line, int Col) ElseChainEnd(List<LineModel> lines, (int Line, int Col) bodyEnd)
    {
        var (line, col) = bodyEnd;
        string content = lines[line].Content;
        string rest = content[(col + 1)..].TrimStart();
        int elseLine;
        int elseCol;

        if (!rest.StartsWith("}") && BracedStatement.MatchElse(rest).Success)
        {
            elseLine = line;
            elseCol = content.IndexOf("else", col + 1, StringComparison.Ordinal) + 4;
        }
        else
        {
            int next = NextCodeLine(lines, line + 1);

            if (next < 0)
            {
                return bodyEnd;
            }

            string trimmed = lines[next].TrimmedContent;

            if (trimmed.StartsWith("}") || !BracedStatement.MatchElse(trimmed).Success)
            {
                return bodyEnd;
            }

            elseLine = next;
            elseCol = lines[next].Content.IndexOf("else", StringComparison.Ordinal) + 4;
        }

        // An else if is a header, so its own else chain is followed too
        return BodyEnd(lines, elseLine, elseCol);
    }

    private static (int Line, int Col) BlockEnd(List<LineModel> lines, int line, int col)
    {
        int depth = 0;

        foreach (var (l, k, c) in CodeChars(lines, line, col))
        {
            if (c == '{')
            {
                depth++;
            }
            else if (c == '}' && --depth == 0)
            {
                return (l, k);
            }
        }

        return (-1, -1);
    }

    /// <summary>
    ///     Finds the ; that ends a statement, outside parentheses, brackets and braces. A statement such as switch
    ///     or try ends with its block instead, unless the code goes on after the }.
    /// </summary>
    private static (int Line, int Col) PlainStatementEnd(List<LineModel> lines, int line, int col)
    {
        string code = lines[line].Content[col..];
        bool isDo = code.StartsWith("do") && (code.Length == 2 || !char.IsLetterOrDigit(code[2]) && code[2] != '_');
        int depth = 0;

        foreach (var (l, k, c) in CodeChars(lines, line, col))
        {
            if (c is '(' or '[' or '{')
            {
                depth++;
            }
            else if (c is ')' or ']' or '}')
            {
                if (--depth < 0)
                {
                    return (-1, -1);
                }

                if (c == '}' && depth == 0 && EndsBlockStatement(lines, l, k, isDo))
                {
                    return (l, k);
                }
            }
            else if (c == ';' && depth == 0)
            {
                return (l, k);
            }
        }

        return (-1, -1);
    }

    /// <summary>
    ///     True when the } at (<paramref name="line"/>, <paramref name="col"/>) ends its statement: nothing follows
    ///     it on its line, and the next code line does not go on with it ("};", ".Method()", "catch", "finally",
    ///     an operator, or the "while" of a do).
    /// </summary>
    private static bool EndsBlockStatement(List<LineModel> lines, int line, int col, bool isDo)
    {
        string after = lines[line].Content[(col + 1)..].Trim();

        if (after.Length > 0 && !after.StartsWith("//"))
        {
            return false;
        }

        int next = NextCodeLine(lines, line + 1);

        if (next < 0)
        {
            return true;
        }

        string trimmed = lines[next].TrimmedContent;

        if (isDo && trimmed.StartsWith("while"))
        {
            return false;
        }

        return !trimmed.StartsWith("catch")
            && !trimmed.StartsWith("finally")
            && trimmed[0] is not ('.' or ';' or ')' or ',' or '?' or ':' or '+' or '-' or '*' or '&' or '|' or '=');
    }

    private static (int Line, int Col) FindConditionEnd(List<LineModel> lines, int line, int col)
    {
        int depth = 0;

        foreach (var (l, k, c) in CodeChars(lines, line, col))
        {
            if (c == '(')
            {
                depth++;
            }
            else if (c == ')' && --depth == 0)
            {
                return (l, k);
            }
        }

        return (-1, -1);
    }

    /// <summary>
    ///     Yields each code character from (<paramref name="line"/>, <paramref name="col"/>) on, skipping string
    ///     and char literals and // comments.
    /// </summary>
    private static IEnumerable<(int Line, int Col, char Char)> CodeChars(List<LineModel> lines, int line, int col)
    {
        bool inString = false;

        for (int j = line; j < lines.Count; j++)
        {
            string lc = lines[j].Content;

            for (int k = j == line ? col : 0; k < lc.Length; k++)
            {
                char c = lc[k];

                if (inString)
                {
                    if (c == '\\')
                    {
                        k++;
                    }
                    else if (c == '"')
                    {
                        inString = false;
                    }

                    continue;
                }

                if (c == '"')
                {
                    inString = true;
                }
                else if (c == '\'')
                {
                    // Char literal: 'x' or '\x'
                    k += k + 1 < lc.Length && lc[k + 1] == '\\' ? 3 : 2;
                }
                else if (c == '/' && k + 1 < lc.Length && lc[k + 1] == '/')
                {
                    break;
                }
                else
                {
                    yield return (j, k, c);
                }
            }
        }
    }

    /// <summary>
    ///     Finds the end of the first statement in a one-line body: the first ; outside parentheses, brackets
    ///     and braces.
    /// </summary>
    private static int FindStatementEnd(string text)
    {
        int depth = 0;
        bool inString = false;

        for (int k = 0; k < text.Length; k++)
        {
            char c = text[k];

            if (inString)
            {
                if (c == '\\')
                {
                    k++;
                }
                else if (c == '"')
                {
                    inString = false;
                }

                continue;
            }

            if (c == '"')
            {
                inString = true;
            }
            else if (c == '(' || c == '{' || c == '[')
            {
                depth++;
            }
            else if (c == ')' || c == '}' || c == ']')
            {
                depth--;
            }
            else if (c == ';' && depth == 0)
            {
                return k;
            }
        }

        return -1;
    }

    /// <summary>
    ///     The first line from <paramref name="from"/> on with code, skipping blank, comment and preprocessor
    ///     lines, or -1.
    /// </summary>
    private static int NextCodeLine(List<LineModel> lines, int from)
    {
        for (int j = from; j < lines.Count; j++)
        {
            string trimmed = lines[j].TrimmedContent;

            if (trimmed.Length > 0 && !trimmed.StartsWith("//") && !trimmed.StartsWith("#"))
            {
                return j;
            }
        }

        return -1;
    }

    private static (int Line, int Col) EndOfLine(List<LineModel> lines, int line) =>
        (line, lines[line].Content.Length - 1);

    private static (int Line, int Col) OrEndOfLine((int Line, int Col) position, List<LineModel> lines, int line) =>
        position.Line < 0 ? EndOfLine(lines, line) : position;

    private static LineModel NewLine(string content, LineEndingEnum lineEnding) =>
        new() { Content = content, TrimmedContent = content.Trim(), LineEnding = lineEnding };

    private static string GetIndent(string content)
    {
        int len = content.Length - content.TrimStart().Length;
        return content[..len];
    }

    private static string LineEndingOf(LineModel line) =>
        line.LineEnding == LineEndingEnum.None ? "\n" : line.LineEnding.ToText();

    private static void AppendLine(StringBuilder sb, LineModel line) =>
        sb.Append(line.Content).Append(line.LineEnding.ToText());
}
