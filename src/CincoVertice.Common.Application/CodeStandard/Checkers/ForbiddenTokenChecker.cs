using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Checkers;

/// <summary>
///     CH0019: a line must not contain a forbidden token anywhere, such as "this.".
/// </summary>
public class ForbiddenTokenChecker : ILineChecker
{
    private static readonly string[] _forbiddenTokens = ["this."];

    public bool Check(LineModel line)
    {
        foreach (var token in _forbiddenTokens)
        {
            if (line.TrimmedContent.Contains(token))
            {
                line.AddError(nameof(Errors.CH0019), Errors.CH0019.Replace("{token}", token));
            }
        }

        return true;
    }
}
