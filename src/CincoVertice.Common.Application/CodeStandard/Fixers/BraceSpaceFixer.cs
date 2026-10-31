using System.Text;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;

namespace CincoVertice.Common.Application.CodeStandard.Fixers;

/// <summary>
///     Fixes CH0033 (see BraceSpaceChecker): adds a space before a { that follows code, and inside braces that have
///     code next to them, so get{return x;} becomes get { return x; } and {} becomes { }. Literals and comments are
///     copied as they are.
/// </summary>
public sealed class BraceSpaceFixer : IStringFixer
{
    public string Fix(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        // FixerPipeline already masks the literals; skipping them here too keeps the fixer safe on its own
        var skipped = CSharpLiteralScanner.FindComments(content)
            .Select(comment => (comment.Start, comment.End))
            .Concat(CSharpLiteralScanner.FindLiterals(content))
            .OrderBy(span => span.Start)
            .ToList();

        var sb = new StringBuilder(content.Length + 16);
        int next = 0;
        int i = 0;

        while (i < content.Length)
        {
            if (next < skipped.Count && i == skipped[next].Start)
            {
                sb.Append(content, i, skipped[next].End - i);
                i = skipped[next].End;
                next++;

                continue;
            }

            char current = content[i];

            if (current == '{')
            {
                if (BraceSpace.NeedsSpaceBeforeOpening(sb.Length > 0 ? sb[^1] : '\n'))
                {
                    sb.Append(' ');
                }

                sb.Append(current);

                if (i + 1 < content.Length && !char.IsWhiteSpace(content[i + 1]))
                {
                    sb.Append(' ');
                }
            }
            else if (current == '}')
            {
                if (sb.Length > 0 && !char.IsWhiteSpace(sb[^1]))
                {
                    sb.Append(' ');
                }

                sb.Append(current);
            }
            else
            {
                sb.Append(current);
            }

            i++;
        }

        return sb.ToString();
    }
}
