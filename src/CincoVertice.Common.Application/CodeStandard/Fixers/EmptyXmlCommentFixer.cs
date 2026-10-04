using System.Text;
using System.Text.RegularExpressions;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Fixers;

public sealed class EmptyXmlCommentFixer : IStringFixer
{
    // /// with nothing after (blank doc line)
    private static readonly Regex _emptyDocLine =
        new(@"^\s*///\s*$", RegexOptions.Compiled);

    // /// containing only a single XML tag: <tag>, </tag>, <tag attr="val">, <tag/>, <tag attr="val"/>
    private static readonly Regex _tagOnlyLine =
        new(@"^\s*///\s*</?[\w:]+(?:\s+[^>]*)?\s*/?>\s*$", RegexOptions.Compiled);

    // /// <tag></tag> or /// <tag attr="val"></tag> — complete empty inline tag pair
    private static readonly Regex _inlineEmptyTag =
        new(@"^\s*///\s*<[\w:]+(?:\s+[^>]*)?\s*></[\w:]+>\s*$", RegexOptions.Compiled);

    /// <summary>
    ///     Removes XML doc comment blocks that contain no meaningful text.
    ///     Within a block that has content, removes individual empty inline tags.
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
            if (IsXmlDocLine(lines[i].Content))
            {
                int start = i;
                while (i < lines.Count && IsXmlDocLine(lines[i].Content))
                {
                    i++;
                }

                var block = lines.GetRange(start, i - start);

                if (BlockHasMeaningfulContent(block))
                {
                    foreach (var line in block)
                    {
                        if (!_inlineEmptyTag.IsMatch(line.Content))
                        {
                            sb.Append(line.Content).Append(line.LineEnding.ToText());
                        }
                    }
                }
                // else: drop the entire empty block
            }
            else
            {
                sb.Append(lines[i].Content).Append(lines[i].LineEnding.ToText());
                i++;
            }
        }

        return sb.ToString();
    }

    private static bool IsXmlDocLine(string content) =>
        content.TrimStart().StartsWith("///", StringComparison.Ordinal);

    private static bool BlockHasMeaningfulContent(List<LineModel> block) =>
        block.Any(line => !_emptyDocLine.IsMatch(line.Content) && !_tagOnlyLine.IsMatch(line.Content));
}