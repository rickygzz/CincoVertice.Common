using System.Text;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Fixers;

/// <summary>
///     Fixes CH0029 (see FileScopedNamespaceChecker): converts the first block namespace to a file-scoped one
///     and de-indents its body. Leaves the file unchanged when the braces are not on their own lines.
/// </summary>
public sealed class FileScopedNamespaceFixer : IStringFixer
{
    private const string Keyword = "namespace ";

    public string Fix(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        var lines = LineParser.GetLines(content);

        int nsIndex = FindNamespaceLine(lines);
        if (nsIndex < 0)
        {
            return content;
        }

        string nsTrimmed = lines[nsIndex].Content.Trim();

        // Already file-scoped, nothing to convert.
        if (nsTrimmed.EndsWith(';'))
        {
            return content;
        }

        string name = ExtractName(nsTrimmed);
        if (name.Length == 0)
        {
            return content;
        }

        int openIndex;
        if (nsTrimmed.EndsWith('{'))
        {
            openIndex = nsIndex;
        }
        else
        {
            openIndex = NextNonBlankLine(lines, nsIndex + 1);

            // A block namespace must be followed by an opening brace on its own line.
            if (openIndex < 0 || lines[openIndex].Content.Trim() != "{")
            {
                return content;
            }
        }

        int closeIndex = FindMatchingBraceLine(lines, openIndex);
        if (closeIndex < 0 || lines[closeIndex].Content.Trim() != "}")
        {
            return content;
        }

        return Rebuild(lines, nsIndex, openIndex, closeIndex, name);
    }

    private static int FindNamespaceLine(List<LineModel> lines)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].Content.TrimStart().StartsWith(Keyword, StringComparison.Ordinal))
            {
                return i;
            }
        }

        return -1;
    }

    private static string ExtractName(string nsTrimmed)
    {
        string remainder = nsTrimmed[Keyword.Length..].TrimStart();

        int end = 0;
        while (end < remainder.Length
            && (char.IsLetterOrDigit(remainder[end]) || remainder[end] == '.' || remainder[end] == '_'))
        {
            end++;
        }

        return remainder[..end];
    }

    private static int NextNonBlankLine(List<LineModel> lines, int start)
    {
        for (int i = start; i < lines.Count; i++)
        {
            if (lines[i].Content.Trim().Length > 0)
            {
                return i;
            }
        }

        return -1;
    }

    private static int FindMatchingBraceLine(List<LineModel> lines, int openIndex)
    {
        int depth = 0;

        for (int i = openIndex; i < lines.Count; i++)
        {
            foreach (char current in lines[i].Content)
            {
                if (current == '{')
                {
                    depth++;
                }
                else if (current == '}')
                {
                    depth--;

                    if (depth == 0)
                    {
                        return i;
                    }
                }
            }
        }

        return -1;
    }

    private static string Rebuild(
        List<LineModel> lines,
        int nsIndex,
        int openIndex,
        int closeIndex,
        string name)
    {
        var sb = new StringBuilder();

        // Everything before the namespace declaration is untouched.
        for (int i = 0; i < nsIndex; i++)
        {
            Append(sb, lines[i]);
        }

        string nsIndent = IndentHelper.GetIndent(lines[nsIndex].Content);
        string nsEnding = lines[nsIndex].LineEnding.ToText();
        sb.Append(nsIndent).Append("namespace ").Append(name).Append(';').Append(nsEnding);

        int bodyStart = openIndex + 1;
        bool hasBody = false;
        for (int i = bodyStart; i < closeIndex; i++)
        {
            if (lines[i].Content.Trim().Length > 0)
            {
                hasBody = true;

                break;
            }
        }

        // Separate the file-scoped declaration from its body with a single blank line.
        if (hasBody && lines[bodyStart].Content.Trim().Length > 0)
        {
            sb.Append(nsEnding);
        }

        // The body is de-indented by one level now that the braces are gone.
        for (int i = bodyStart; i < closeIndex; i++)
        {
            sb.Append(Deindent(lines[i].Content)).Append(lines[i].LineEnding.ToText());
        }

        // Everything after the closing brace is untouched (the brace line itself is dropped).
        for (int i = closeIndex + 1; i < lines.Count; i++)
        {
            Append(sb, lines[i]);
        }

        return sb.ToString();
    }

    private static void Append(StringBuilder sb, LineModel line)
    {
        sb.Append(line.Content).Append(line.LineEnding.ToText());
    }

    private static string Deindent(string content)
    {
        int remove = 0;
        while (remove < 4 && remove < content.Length && content[remove] == ' ')
        {
            remove++;
        }

        if (remove == 0 && content.Length > 0 && content[0] == '\t')
        {
            return content[1..];
        }

        return content[remove..];
    }
}
