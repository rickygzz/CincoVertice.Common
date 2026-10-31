using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Checkers;

/// <summary>
///     CH0034: block braces go on their own lines. A line must not end with a { after code ("if (x) {"), and a }
///     must not be followed by a statement ("} else"). "});", "} while (x);" and single-line blocks such as
///     get { return x; } are allowed. Fixed by BracePlacementFixer.
///     <para>Runs after the comment checker, which removes a trailing // comment from
///     <see cref="LineModel.TrimmedContent"/>.</para>
/// </summary>
public class BracePlacementChecker : ILineChecker
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

        if (BracePlacement.ClosingBraceHasCodeAfter(code)
            || BracePlacement.OpeningBraceHasCodeBefore(code) && !EndsInsideLiteral(code))
        {
            line.AddError(nameof(Errors.CH0034), Errors.CH0034);
        }

        return true;
    }

    /// <summary>
    ///     True when the line ends inside a literal, as in var s = @"{ starting a multi-line string.
    /// </summary>
    private static bool EndsInsideLiteral(string code)
    {
        int i = 0;

        while (i < code.Length)
        {
            if (code[i] is '"' or '\'' or '$' or '@')
            {
                int end = CSharpLiteralScanner.SkipLiteral(code, i);

                if (end >= code.Length && end > i + 1)
                {
                    return true;
                }

                i = end;
            }
            else
            {
                i++;
            }
        }

        return false;
    }
}
