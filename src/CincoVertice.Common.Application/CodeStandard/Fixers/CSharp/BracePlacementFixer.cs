using System.Text;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Fixers.CSharp;

/// <summary>
///     Fixes CH0034 (see BracePlacementChecker): puts block braces on their own lines, as Visual Studio formats C#.
///     "if (x) {" becomes "if (x)" and "{"; "} else {" becomes "}", "else" and "{". New lines use the line's own
///     indentation and line ending. A trailing comment stays with the code before the brace.
/// </summary>
public sealed class BracePlacementFixer : IStringFixer
{
    public string Fix(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        var lines = LineParser.GetLines(content);
        var spans = LineSpans.Find(content, lines);

        // For splitting the last line, which has no line ending of its own
        string newLine = lines.FirstOrDefault(line => line.LineEnding != LineEndingEnum.None)?.LineEnding.ToText()
            ?? "\n";

        var sb = new StringBuilder(content.Length + 64);

        for (int i = 0; i < lines.Count; i++)
        {
            string ending = lines[i].LineEnding.ToText();

            if (spans[i].StartsInside || spans[i].EndsInside)
            {
                sb.Append(lines[i].Content).Append(ending);
            }
            else
            {
                AppendFixed(sb, lines[i].Content, ending, ending.Length > 0 ? ending : newLine);
            }
        }

        return sb.ToString();
    }

    private static void AppendFixed(StringBuilder sb, string lineContent, string ending, string splitEnding)
    {
        string indent = IndentHelper.GetIndent(lineContent);
        int codeEnd = LineSpans.CodeEnd(lineContent);
        string code = lineContent[..codeEnd].Trim();
        string comment = lineContent[codeEnd..].Trim();
        string trailingComment = comment.Length > 0 ? " " + comment : string.Empty;

        if (BracePlacement.ClosingBraceHasCodeAfter(code))
        {
            sb.Append(indent).Append('}').Append(splitEnding);

            // The rest may itself end with a brace: "} else {"
            AppendFixed(sb, indent + code[1..].TrimStart() + trailingComment, ending, splitEnding);
        }
        else if (BracePlacement.OpeningBraceHasCodeBefore(code))
        {
            sb.Append(indent).Append(code[..^1].TrimEnd()).Append(trailingComment).Append(splitEnding);
            sb.Append(indent).Append('{').Append(ending);
        }
        else
        {
            sb.Append(lineContent).Append(ending);
        }
    }
}
