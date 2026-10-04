using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Checkers;

/// <summary>
///     CH0010: a line must not contain only whitespace. CH0011: a line must not end with whitespace.
///     Both are fixed by TrailingWhitespaceFixer, which empties a whitespace-only line.
///     <para>A whitespace-only line stops parsing, since there is nothing else to check.</para>
/// </summary>
public class TrailingWhitespaceChecker : ILineChecker
{
    /// <inheritdoc/>
    public bool Check(LineModel line)
    {
        if (line.Content.Length == 0 || !char.IsWhiteSpace(line.Content[^1]))
        {
            return true;
        }

        if (string.IsNullOrEmpty(line.TrimmedContent))
        {
            line.AddError(nameof(Errors.CH0010), Errors.CH0010);

            return false;
        }

        line.AddError(nameof(Errors.CH0011), Errors.CH0011);

        return true;
    }
}
