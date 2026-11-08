using System.Text;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;

namespace CincoVertice.Common.Application.CodeStandard.Fixers;

/// <summary>
///     Puts a space after // and /// (CH0020, CH0021; see CommentSpaceChecker), in comment lines and trailing comments:
///     "//comment" becomes "// comment" and "///&lt;summary&gt;" becomes "/// &lt;summary&gt;".
///     A tab right after the slashes is replaced by the space.
///     <para>Left unchanged: // inside literals, /* */ comments, empty comments, and four or more slashes
///     (e.g. separator lines).</para>
/// </summary>
public sealed class CommentSpaceFixer : IStringFixer
{
    public string Fix(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        var sb = new StringBuilder(content.Length + 16);
        int copied = 0;

        foreach (var comment in CSharpLiteralScanner.FindComments(content))
        {
            if (comment.IsBlock)
            {
                continue;
            }

            int slashes = CountSlashes(content, comment.Start, comment.End);
            int textStart = comment.Start + slashes;

            if (slashes > 3 || textStart >= comment.End || content[textStart] == ' ')
            {
                continue;
            }

            sb.Append(content, copied, textStart - copied);
            sb.Append(' ');

            // Replace a tab (or other whitespace) after the slashes; otherwise keep the text and insert the space
            copied = char.IsWhiteSpace(content[textStart]) ? textStart + 1 : textStart;
        }

        sb.Append(content, copied, content.Length - copied);

        return sb.ToString();
    }

    private static int CountSlashes(string content, int start, int end)
    {
        int i = start;

        while (i < end && content[i] == '/')
        {
            i++;
        }

        return i - start;
    }
}
