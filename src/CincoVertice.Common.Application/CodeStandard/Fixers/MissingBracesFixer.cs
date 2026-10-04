using System.Text;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Fixers;

/// <summary>
///     Fixes CH0031 (see MissingBracesChecker).
/// </summary>
public sealed class MissingBracesFixer : IStringFixer
{
    /// <summary>
    ///     Adds braces to <c>if</c>, <c>else if</c>, and <c>else</c> blocks that are missing them,
    ///     with each brace on its own line.
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
        int i = 0;

        while (i < lines.Count)
        {
            string trimmed = lines[i].TrimmedContent;

            if (IfElseStatement.MatchIf(trimmed).Success)
            {
                i = ProcessIfOrElseIf(lines, i, sb);
            }
            else if (IfElseStatement.MatchElse(trimmed).Success)
            {
                i = ProcessBareElse(lines, i, sb);
            }
            else
            {
                AppendLine(sb, lines[i]);
                i++;
            }
        }

        return sb.ToString();
    }

    private static int ProcessIfOrElseIf(List<LineModel> lines, int i, StringBuilder sb)
    {
        string indent = GetIndent(lines[i].Content);
        string le = LineEndingOf(lines[i]);

        var (endLine, endCol) = FindConditionEnd(lines, i);

        if (endLine < 0)
        {
            AppendLine(sb, lines[i]);
            return i + 1;
        }

        var condLines = lines.GetRange(i, endLine - i + 1);
        string lastContent = condLines.Last().Content;
        string afterParen = lastContent[(endCol + 1)..].TrimStart();

        if (afterParen.StartsWith("{"))
        {
            foreach (var cl in condLines)
                AppendLine(sb, cl);
            return endLine + 1;
        }

        if (!string.IsNullOrWhiteSpace(afterParen) && !afterParen.StartsWith("//"))
        {
            // Single-line: if (cond) body;
            for (int ci = 0; ci < condLines.Count - 1; ci++)
                AppendLine(sb, condLines[ci]);

            sb.Append(lastContent[..(endCol + 1)]).Append(le);
            sb.Append(indent).Append("{").Append(le);
            sb.Append(indent).Append("    ").Append(afterParen).Append(le);
            sb.Append(indent).Append("}").Append(le);
            return endLine + 1;
        }

        // Body on next line
        foreach (var cl in condLines)
            AppendLine(sb, cl);

        return ProcessBodyOnNextLine(lines, endLine + 1, indent, le, sb);
    }

    private static int ProcessBareElse(List<LineModel> lines, int i, StringBuilder sb)
    {
        string indent = GetIndent(lines[i].Content);
        string le = LineEndingOf(lines[i]);
        string trimmed = lines[i].TrimmedContent;

        int elseEnd = trimmed.IndexOf("else", StringComparison.Ordinal) + 4;
        string afterElse = trimmed[elseEnd..].TrimStart();

        if (afterElse.StartsWith("{"))
        {
            AppendLine(sb, lines[i]);
            return i + 1;
        }

        if (!string.IsNullOrWhiteSpace(afterElse) && !afterElse.StartsWith("//"))
        {
            // Single-line: else body; (or } else body;)
            int elseInContent = lines[i].Content.IndexOf("else", StringComparison.Ordinal);
            sb.Append(lines[i].Content[..(elseInContent + 4)]).Append(le);
            sb.Append(indent).Append("{").Append(le);
            sb.Append(indent).Append("    ").Append(afterElse).Append(le);
            sb.Append(indent).Append("}").Append(le);
            return i + 1;
        }

        AppendLine(sb, lines[i]);
        return ProcessBodyOnNextLine(lines, i + 1, indent, le, sb);
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

        int bodyEnd = FindBodyEnd(lines, i);
        if (bodyEnd < 0) bodyEnd = i;

        sb.Append(indent).Append("{").Append(le);
        for (int j = i; j <= bodyEnd; j++)
            AppendLine(sb, lines[j]);
        sb.Append(indent).Append("}").Append(LineEndingOf(lines[bodyEnd]));

        return bodyEnd + 1;
    }

    private static int FindBodyEnd(List<LineModel> lines, int start)
    {
        int braceDepth = 0;
        int parenDepth = 0;
        bool inString = false;

        for (int j = start; j < lines.Count; j++)
        {
            string lc = lines[j].Content;

            for (int k = 0; k < lc.Length; k++)
            {
                char c = lc[k];

                if (inString)
                {
                    if (c == '\\') k++;
                    else if (c == '"') inString = false;
                    continue;
                }

                if (c == '"') { inString = true; continue; }

                if (c == '(') parenDepth++;
                else if (c == ')') parenDepth--;
                else if (c == '{') braceDepth++;
                else if (c == '}') braceDepth--;
                else if (c == ';' && braceDepth == 0 && parenDepth == 0)
                    return j;
            }
        }

        return -1;
    }

    private static (int endLine, int endCol) FindConditionEnd(List<LineModel> lines, int startLine)
    {
        int depth = 0;
        bool inString = false;

        for (int j = startLine; j < lines.Count; j++)
        {
            string lc = lines[j].Content;

            for (int k = 0; k < lc.Length; k++)
            {
                char c = lc[k];

                if (inString)
                {
                    if (c == '\\') k++;
                    else if (c == '"') inString = false;

                    continue;
                }

                if (c == '"') { inString = true; continue; }
                if (c == '(') depth++;
                else if (c == ')' && --depth == 0)
                    return (j, k);
            }
        }

        return (-1, -1);
    }

    private static string GetIndent(string content)
    {
        int len = content.Length - content.TrimStart().Length;
        return content[..len];
    }

    private static string LineEndingOf(LineModel line) =>
        line.LineEnding == LineEndingEnum.None ? "\n" : line.LineEnding.ToText();

    private static void AppendLine(StringBuilder sb, LineModel line) =>
        sb.Append(line.Content).Append(line.LineEnding.ToText());}
