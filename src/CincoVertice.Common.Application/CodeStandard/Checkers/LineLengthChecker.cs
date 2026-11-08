using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Checkers;

/// <summary>
///     CH0016: a line must not be longer than <see cref="MaxLength"/> characters.
/// </summary>
public class LineLengthChecker : ILineChecker
{
    public const int MaxLength = 120;

    public bool Check(LineModel line)
    {
        if (line.Content.Length > MaxLength)
        {
            line.AddError(nameof(Errors.CH0016), Errors.CH0016);
        }

        return true;
    }
}
