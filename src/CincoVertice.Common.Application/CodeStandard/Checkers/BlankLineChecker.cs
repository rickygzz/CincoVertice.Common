using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Checkers;

/// <summary>
///     CH0027: no more than one blank line in a row. CH0028: no blank line before a closing brace, reported on
///     the } line. A blank line is empty or whitespace only. Fixed by BlankLineFixer.
///     <para>An empty line stops parsing, since there is nothing else to check. A whitespace-only line continues,
///     so it also gets CH0010.</para>
/// </summary>
public class BlankLineChecker : ILineChecker
{
    /// <inheritdoc/>
    public bool Check(LineModel line)
    {
        if (string.IsNullOrWhiteSpace(line.Content))
        {
            if (line.PreviousLineIsBlank)
            {
                line.AddError(nameof(Errors.CH0027), Errors.CH0027);
            }

            return line.Content.Length > 0;
        }

        if (line.PreviousLineIsBlank && line.TrimmedContent.StartsWith('}'))
        {
            line.AddError(nameof(Errors.CH0028), Errors.CH0028);
        }

        return true;
    }
}
