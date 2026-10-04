using System.Text.RegularExpressions;
using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Checkers;

/// <summary>
///     CH0017: a line must not start with an operator or separator; it belongs at the end of the previous line.
///     A line starting with a lambda parameter list, such as "(a, b) =>", is allowed.
/// </summary>
public partial class ForbiddenStartTokenChecker : ILineChecker
{
    private static readonly string[] _forbiddenStartTokens =
        [",", ";", ")", "(", "=", "+=", "-=", "*=", "/=", "%=", "==", "!=", ">=", "<=", "=>", "<", ">"];

    public bool Check(LineModel line)
    {
        foreach (var token in _forbiddenStartTokens)
        {
            if (line.TrimmedContent.StartsWith(token))
            {
                if (token.Equals("(") && LambdaParameterListRegex().IsMatch(line.TrimmedContent))
                {
                    break;
                }

                line.AddError(nameof(Errors.CH0017), Errors.CH0017.Replace("{token}", token));
                break;
            }
        }

        return true;
    }

    [GeneratedRegex(@"\(\s*(\w+\s*(,\s*\w+\s*)*)?\)\s*([=>]|=)")]
    private static partial Regex LambdaParameterListRegex();
}
