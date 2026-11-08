using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Checkers.CSharp;

/// <summary>
///     CH0018: a line must not end with a binary operator; it belongs at the start of the next line.
///     "case x:", "default:" and a "!" that is not a separate operator (e.g. "value!") are allowed.
/// </summary>
public class ForbiddenEndTokenChecker : ILineChecker
{
    private static readonly string[] _forbiddenEndTokens =
        ["&&", "||", "??=", "??", "!", "?", ":", "+", "-", "/", "*"];

    public bool Check(LineModel line)
    {
        // The end of a line inside a multi-line string is text, not an operator
        if (line.StartsInsideLiteral)
        {
            return true;
        }

        foreach (var token in _forbiddenEndTokens)
        {
            if (line.TrimmedContent.EndsWith(token))
            {
                if (token.Equals(":")
                    && (line.TrimmedContent.StartsWith("case")
                    || line.TrimmedContent.StartsWith("default")))
                {
                    break;
                }

                if (token.Equals("!") && !line.TrimmedContent.EndsWith(" !"))
                {
                    break;
                }

                line.AddError(nameof(Errors.CH0018), Errors.CH0018.Replace("{token}", token));

                break;
            }
        }

        return true;
    }
}
