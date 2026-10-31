using System.Text;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;

namespace CincoVertice.Common.Application.CodeStandard.Fixers;

/// <summary>
///     Fixes CH0035 (see ParenthesisSpaceChecker): removes spaces after ( and before ), so Read( a ) becomes
///     Read(a) and ( ) becomes (). Spaces after a ( that ends its line, and the indentation before a ) that starts
///     its line, are kept. Literals and comments are copied as they are.
/// </summary>
public sealed class ParenthesisSpaceFixer : IStringFixer
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
            else if (current == ')')
            {
                // A ) starting its line keeps the indentation before it
                while (afterCode && sb.Length > 0 && sb[^1] is ' ' or '\t')
                {
                    sb.Length--;
                }

                afterCode = true;
            }
            else if (!char.IsWhiteSpace(current))
            {
                afterCode = true;
            }

            sb.Append(current);
            i++;

            if (current == '(')
            {
                int run = i;

                while (run < content.Length && content[run] is ' ' or '\t')
                {
                    run++;
                }

                // Keep the spaces before a line end (TrailingWhitespaceFixer's job) or a comment
                bool beforeComment = next < skipped.Count && skipped[next].Start == run
                    && content[run] == '/';

                if (run < content.Length && content[run] is not ('\r' or '\n') && !beforeComment)
                {
                    i = run;
                }
            }
        }

        return sb.ToString();
    }
}
