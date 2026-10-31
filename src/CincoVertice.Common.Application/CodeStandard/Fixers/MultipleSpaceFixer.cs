using System.Text;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;

namespace CincoVertice.Common.Application.CodeStandard.Fixers;

/// <summary>
///     Fixes CH0032 (see MultipleSpaceChecker): replaces consecutive spaces in code with one space. Indentation,
///     spaces at the end of a line (left to TrailingWhitespaceFixer) and spaces before a comment are kept, and
///     literals and comments are copied as they are.
/// </summary>
public sealed class MultipleSpaceFixer : IStringFixer
{
    public string Fix(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        // FixerPipeline already masks the literals; skipping them here too keeps the fixer safe on its own
        var skipped = CSharpLiteralScanner.FindComments(content)
            .Select(comment => (comment.Start, comment.End, IsComment: true))
            .Concat(CSharpLiteralScanner.FindLiterals(content)
                .Select(literal => (literal.Start, literal.End, IsComment: false)))
            .OrderBy(span => span.Start)
            .ToList();

        var sb = new StringBuilder(content.Length);
        int next = 0;
        bool afterCode = false;
        int i = 0;

        while (i < content.Length)
        {
            if (next < skipped.Count && i == skipped[next].Start)
            {
                sb.Append(content, i, skipped[next].End - i);
                i = skipped[next].End;
                next++;
                afterCode = true;

                continue;
            }

            char current = content[i];

            if (current is '\r' or '\n')
            {
                afterCode = false;
            }
            else if (current == ' ')
            {
                int run = i;

                while (run < content.Length && content[run] == ' ')
                {
                    run++;
                }

                bool atLineEnd = run == content.Length || content[run] is '\r' or '\n';
                bool beforeComment = next < skipped.Count && skipped[next].IsComment && skipped[next].Start == run;

                if (afterCode && run - i > 1 && !atLineEnd && !beforeComment)
                {
                    sb.Append(' ');
                }
                else
                {
                    sb.Append(content, i, run - i);
                }

                i = run;

                continue;
            }
            else if (!char.IsWhiteSpace(current))
            {
                afterCode = true;
            }

            sb.Append(current);
            i++;
        }

        return sb.ToString();
    }
}
