using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Checkers;

/// <summary>
///     CH0031: if, else if and else bodies must be in braces. Fixed by MissingBracesFixer.
///     <para>A body on the same line (if (x) return;) is reported on the if line. A body on the next line is
///     reported on that line, using the state this checker leaves on <see cref="LineModel.PreviousCodeLine"/>.
///     Conditions spanning several lines are followed.</para>
/// </summary>
public class MissingBracesChecker : ILineChecker
{
    /// <inheritdoc/>
    public bool Check(LineModel line)
    {
        string code = line.TrimmedContent;
        var previous = line.PreviousCodeLine;

        if (previous?.OpenConditionParens > 0)
        {
            CheckCondition(line, code, 0, previous.OpenConditionParens);

            return true;
        }

        if (previous?.AwaitsOpeningBrace == true && !code.StartsWith('{'))
        {
            line.AddError(nameof(Errors.CH0031), Errors.CH0031);
        }

        var ifMatch = IfElseStatement.MatchIf(code);

        if (ifMatch.Success)
        {
            CheckCondition(line, code, ifMatch.Length, 1);

            return true;
        }

        var elseMatch = IfElseStatement.MatchElse(code);

        if (elseMatch.Success)
        {
            CheckBody(line, code[elseMatch.Length..]);
        }

        return true;
    }

    /// <summary>
    ///     Finds the parenthesis that closes the condition, then checks what follows it. When the line ends first,
    ///     the open count is left for the next code line.
    /// </summary>
    private static void CheckCondition(LineModel line, string code, int start, int depth)
    {
        int i = start;

        while (i < code.Length)
        {
            char current = code[i];

            if (current is '"' or '\'' or '$' or '@')
            {
                // A ) inside a literal does not close the condition
                i = CSharpLiteralScanner.SkipLiteral(code, i);

                continue;
            }

            if (current == '(')
            {
                depth++;
            }
            else if (current == ')' && --depth == 0)
            {
                CheckBody(line, code[(i + 1)..]);

                return;
            }

            i++;
        }

        line.OpenConditionParens = depth;
    }

    private static void CheckBody(LineModel line, string afterHeader)
    {
        string body = afterHeader.Trim();

        if (body.Length == 0)
        {
            line.AwaitsOpeningBrace = true;
        }
        else if (!body.StartsWith('{'))
        {
            line.AddError(nameof(Errors.CH0031), Errors.CH0031);
        }
    }
}
