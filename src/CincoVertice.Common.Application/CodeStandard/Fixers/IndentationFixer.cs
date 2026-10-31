using System.Text;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;

namespace CincoVertice.Common.Application.CodeStandard.Fixers;

public sealed class IndentationFixer : IStringFixer
{
    private const int SpacesPerIndent = 4;

    public string Fix(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        var lines = LineParser.GetLines(content);
        var sb = new StringBuilder(content.Length);

        // The open indentation levels, as (level in the input, level in the output). A line may be at most one
        // level deeper than the line before it (CH0014), so a deeper jump is closed up, and the lines nested
        // under it move up with it. The first code line, or one less indented than every line before it, keeps
        // its level.
        var levels = new Stack<(int Original, int Fixed)>();

        foreach (var line in lines)
        {
            sb.Append(FixIndentation(line.Content, levels));
            sb.Append(line.LineEnding.ToText());
        }

        return sb.ToString();
    }

    private static string FixIndentation(string lineContent, Stack<(int Original, int Fixed)> levels)
    {
        int columns = 0;
        int i = 0;

        while (i < lineContent.Length)
        {
            char current = lineContent[i];

            if (current == '\t')
            {
                columns += SpacesPerIndent;
            }
            else if (current == ' ')
            {
                columns++;
            }
            else
            {
                break;
            }

            i++;
        }

        // Whitespace-only lines carry no code to indent; leave them for the trailing-whitespace fixer.
        if (i == lineContent.Length)
        {
            return lineContent;
        }

        // Round the indentation width to the nearest multiple of four.
        int level = (columns + SpacesPerIndent / 2) / SpacesPerIndent;

        while (levels.Count > 0 && levels.Peek().Original > level)
        {
            levels.Pop();
        }

        int fixedLevel = level;

        if (levels.Count == 0)
        {
            levels.Push((level, level));
        }
        else if (levels.Peek().Original == level)
        {
            fixedLevel = levels.Peek().Fixed;
        }
        else
        {
            fixedLevel = levels.Peek().Fixed + 1;
            levels.Push((level, fixedLevel));
        }

        return new string(' ', fixedLevel * SpacesPerIndent) + lineContent[i..];
    }
}
