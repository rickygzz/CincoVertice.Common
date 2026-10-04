using System.Text;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Fixers;

/// <summary>
///     Fixes CH0027 and CH0028 (see BlankLineChecker).
/// </summary>
public sealed class BlankLineFixer : IStringFixer
{
    /// <summary>
    ///     Reduces any run of two or more consecutive blank lines to a single blank line.
    ///     Removes any blank lines immediately before a closing brace <c>}</c>.
    ///     A single blank line between code blocks is preserved.
    /// </summary>
    /// <param name="content">The file content to fix.</param>
    /// <returns>The content with consecutive blank lines reduced and no blank lines before <c>}</c>.</returns>
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
            if (string.IsNullOrWhiteSpace(lines[i].Content))
            {
                int blankStart = i;
                while (i < lines.Count && string.IsNullOrWhiteSpace(lines[i].Content))
                {
                    i++;
                }

                bool beforeClosingBrace = i < lines.Count
                    && lines[i].TrimmedContent.StartsWith("}");

                if (!beforeClosingBrace)
                {
                    AppendLine(sb, lines[blankStart]);
                }
            }
            else
            {
                AppendLine(sb, lines[i]);
                i++;
            }
        }

        return sb.ToString();
    }

    private static void AppendLine(StringBuilder sb, LineModel line) =>
        sb.Append(line.Content).Append(line.LineEnding.ToText());
}
