using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Checkers.CSharp;

/// <summary>
///     CH0033: a { after code needs a space before it, and braces with code next to them need a space inside:
///     get { return x; }, and { } when empty. Braces alone on their line, literals and comments are not checked.
///     Fixed by BraceSpaceFixer.
///     <para>Runs after the comment checker, which removes a trailing // comment from
///     <see cref="LineModel.TrimmedContent"/>, so only the code is checked.</para>
/// </summary>
public class BraceSpaceChecker : ILineChecker
{
    /// <inheritdoc/>
    public bool Check(LineModel line)
    {
        string code = line.TrimmedContent;

        // Lines of a multi-line string or of a /* */ comment
        if (line.StartsInsideLiteral || code.StartsWith("/*") || code.StartsWith('*'))
        {
            return true;
        }

        int i = 0;

        while (i < code.Length)
        {
            char current = code[i];

            if (current is '"' or '\'' or '$' or '@')
            {
                i = CSharpLiteralScanner.SkipLiteral(code, i);

                continue;
            }

            if (current == '/' && i + 1 < code.Length && code[i + 1] == '*')
            {
                int end = code.IndexOf("*/", i + 2, StringComparison.Ordinal);

                if (end < 0)
                {
                    return true;
                }

                i = end + 2;

                continue;
            }

            if (IsMissingSpace(code, i))
            {
                line.AddError(nameof(Errors.CH0033), Errors.CH0033);

                return true;
            }

            i++;
        }

        return true;
    }

    private static bool IsMissingSpace(string code, int i)
    {
        if (code[i] == '{')
        {
            bool spaceBefore = i == 0 || !BraceSpace.NeedsSpaceBeforeOpening(code[i - 1]);
            bool spaceAfter = i + 1 == code.Length || code[i + 1] == ' ';

            return !spaceBefore || !spaceAfter;
        }

        return code[i] == '}' && i > 0 && code[i - 1] != ' ';
    }
}
