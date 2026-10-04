using System.Text;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Fixers;

/// <summary>
///     Fixes CH0030 (empty XML comments, see EmptyXmlCommentChecker): removes empty tag pairs such as
///     &lt;returns&gt;&lt;/returns&gt;, drops a line left with no text, and drops a whole /// block that has no text.
/// </summary>
public sealed class EmptyXmlCommentFixer : IStringFixer
{
    /// <summary>
    ///     Removes XML doc comment blocks that contain no meaningful text.
    ///     Within a block that has content, removes individual empty tags.
    /// </summary>
    /// <param name="content">The file content to fix.</param>
    /// <returns>The content with empty XML doc comments removed.</returns>
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
            if (!XmlDocComment.IsDocLine(lines[i].Content))
            {
                sb.Append(lines[i].Content).Append(lines[i].LineEnding.ToText());
                i++;

                continue;
            }

            var block = new List<LineModel>();

            for (; i < lines.Count && XmlDocComment.IsDocLine(lines[i].Content); i++)
            {
                var cleaned = RemoveEmptyTags(lines[i]);

                if (cleaned is not null)
                {
                    block.Add(cleaned);
                }
            }

            // Drop the entire block when no line has text
            if (block.Any(line => !XmlDocComment.IsWithoutText(line.Content)))
            {
                foreach (var line in block)
                {
                    sb.Append(line.Content).Append(line.LineEnding.ToText());
                }
            }
        }

        return sb.ToString();
    }

    /// <summary>
    ///     Returns the line without its empty tags, or null when only an empty tag was on it.
    /// </summary>
    private static LineModel? RemoveEmptyTags(LineModel line)
    {
        if (!XmlDocComment.ContainsEmptyTag(line.Content))
        {
            return line;
        }

        string cleaned = XmlDocComment.RemoveEmptyTags(line.Content).TrimEnd();

        if (cleaned.TrimStart() == "///")
        {
            return null;
        }

        return new LineModel { Content = cleaned, LineEnding = line.LineEnding };
    }
}
