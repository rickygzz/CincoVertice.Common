using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Checkers;

/// <summary>
///     CH0035: no space after ( or before ): Read(a), not Read( a ). A ( ending its line and a ) starting it are
///     allowed, and literals and comments are not checked. Fixed by ParenthesisSpaceFixer.
///     <para>Runs after the comment checker, which removes a trailing // comment from
///     <see cref="LineModel.TrimmedContent"/>.</para>
/// </summary>
public class ParenthesisSpaceChecker : ILineChecker
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

            bool spaceAfterOpening = current == '(' && i + 1 < code.Length && code[i + 1] is ' ' or '\t'
                && !code[(i + 1)..].TrimStart().StartsWith("/*");
            bool spaceBeforeClosing = current == ')' && i > 0 && code[i - 1] is ' ' or '\t';

            if (spaceAfterOpening || spaceBeforeClosing)
            {
                line.AddError(nameof(Errors.CH0035), Errors.CH0035);

                return true;
            }

            i++;
        }

        return true;
    }
}
