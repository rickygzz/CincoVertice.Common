using System.Text;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;

namespace CincoVertice.Common.Application.CodeStandard.Fixers;

public sealed class TrailingOperatorFixer : IStringFixer
{
    // Longest operators first so '??=' wins over '??' over '?'.
    private static readonly string[] Operators =
        ["??=", "&&", "||", "??", "?", ":", "+", "-", "/", "*"];

    public string Fix(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        var lines = LineParser.GetLines(content);
        var contents = new string[lines.Count];
        for (int i = 0; i < lines.Count; i++)
        {
            contents[i] = lines[i].Content;
        }

        bool inBlockComment = false;

        // The last line has nowhere to move a trailing operator to, so it is left untouched.
        for (int i = 0; i < contents.Length; i++)
        {
            inBlockComment = ScanCommentState(contents[i], inBlockComment, out bool endsInComment);

            if (i >= contents.Length - 1)
            {
                break;
            }

            // The trailing operator is inside a comment, so it is not really a trailing operator.
            if (endsInComment)
            {
                continue;
            }

            // Do not move an operator onto a line that begins a comment.
            string nextLeading = contents[i + 1].TrimStart();
            if (nextLeading.StartsWith("//", StringComparison.Ordinal)
                || nextLeading.StartsWith("/*", StringComparison.Ordinal))
            {
                continue;
            }

            string trimmedEnd = contents[i].TrimEnd(' ', '\t');

            string? op = MatchTrailingOperator(trimmedEnd);
            if (op is null)
            {
                continue;
            }

            // A ':' ending a switch label is not a ternary/loop operator.
            if (op == ":")
            {
                string leading = contents[i].TrimStart();

                if (leading.StartsWith("case", StringComparison.Ordinal)
                    || leading.StartsWith("default", StringComparison.Ordinal))
                {
                    continue;
                }
            }

            // Drop the operator (and any space before it) from the current line.
            contents[i] = trimmedEnd[..^op.Length].TrimEnd(' ', '\t');

            // Move it to the beginning of the next line's content, keeping that line's indentation.
            string next = contents[i + 1];
            string indent = IndentHelper.GetIndent(next);
            string rest = next[indent.Length..];

            contents[i + 1] = rest.Length == 0
                ? indent + op
                : $"{indent}{op} {rest}";
        }

        var sb = new StringBuilder(content.Length);
        for (int i = 0; i < lines.Count; i++)
        {
            sb.Append(contents[i]).Append(lines[i].LineEnding.ToText());
        }

        return sb.ToString();
    }

    /// <summary>
    ///     Scans a single line for comments, returning the block-comment state at the end of the
    ///     line and whether the end of the line falls inside a comment. String literals are not
    ///     tracked, matching the naive comment handling of the other fixers.
    /// </summary>
    private static bool ScanCommentState(string line, bool inBlockComment, out bool endsInComment)
    {
        int i = 0;
        while (i < line.Length)
        {
            if (inBlockComment)
            {
                if (line[i] == '*' && i + 1 < line.Length && line[i + 1] == '/')
                {
                    inBlockComment = false;
                    i += 2;
                }
                else
                {
                    i++;
                }
            }
            else if (line[i] == '/' && i + 1 < line.Length && line[i + 1] == '/')
            {
                endsInComment = true;

                return false;
            }
            else if (line[i] == '/' && i + 1 < line.Length && line[i + 1] == '*')
            {
                inBlockComment = true;
                i += 2;
            }
            else
            {
                i++;
            }
        }

        endsInComment = inBlockComment;

        return inBlockComment;
    }

    private static string? MatchTrailingOperator(string trimmedEnd)
    {
        if (trimmedEnd.Length == 0)
        {
            return null;
        }

        // Avoid mangling comment delimiters that happen to end with an operator character.
        if (trimmedEnd.EndsWith("//", StringComparison.Ordinal)
            || trimmedEnd.EndsWith("/*", StringComparison.Ordinal)
            || trimmedEnd.EndsWith("*/", StringComparison.Ordinal))
        {
            return null;
        }

        foreach (var op in Operators)
        {
            if (trimmedEnd.EndsWith(op, StringComparison.Ordinal))
            {
                return op;
            }
        }

        return null;
    }
}
