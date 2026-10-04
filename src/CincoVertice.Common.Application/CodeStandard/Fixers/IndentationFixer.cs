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

        foreach (var line in lines)
        {
            sb.Append(FixIndentation(line.Content));
            sb.Append(line.LineEnding.ToText());
        }

        return sb.ToString();
    }

    private static string FixIndentation(string lineContent)
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
        int indentLevels = (columns + SpacesPerIndent / 2) / SpacesPerIndent;

        return new string(' ', indentLevels * SpacesPerIndent) + lineContent[i..];
    }
}
