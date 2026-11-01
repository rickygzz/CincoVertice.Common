using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Checkers;

/// <summary>
///     CH0017: a line must not start with an operator or separator; it belongs at the end of the previous line.
///     A line may start with "(", because it usually starts a new expression: a lambda "(a, b) =>", a cast, a
///     tuple pattern "(1, 'a') =>" or a parenthesized operand. ForbiddenStartTokenFixer leaves it too.
/// </summary>
public class ForbiddenStartTokenChecker : ILineChecker
{
    private static readonly string[] _forbiddenStartTokens =
        [",", ";", ")", "=", "+=", "-=", "*=", "/=", "%=", "==", "!=", ">=", "<=", "=>", "<", ">"];

    public bool Check(LineModel line)
    {
        // A line of a multi-line string is not code
        if (line.StartsInsideLiteral)
        {
            return true;
        }

        foreach (var token in _forbiddenStartTokens)
        {
            if (line.TrimmedContent.StartsWith(token))
            {
                line.AddError(nameof(Errors.CH0017), Errors.CH0017.Replace("{token}", token));
                break;
            }
        }

        return true;
    }
}
