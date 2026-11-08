using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Checkers.JavaScript;

/// <summary>
///     CH0036: a line must be indented as far as the brackets around it say, in
///     <see cref="LineModel.ExpectedIndentation"/>, with spaces. Fixed by BraceIndentationFixer.
///     <para>Takes the place of IndentationChecker in languages indented this way, such as JavaScript.</para>
/// </summary>
public class BraceIndentationChecker : ILineChecker
{
    /// <inheritdoc/>
    public bool Check(LineModel line)
    {
        if (line.ExpectedIndentation is not { } expected || string.IsNullOrWhiteSpace(line.Content))
        {
            return true;
        }

        string indent = line.Content[..(line.Content.Length - line.Content.TrimStart().Length)];

        if (indent.Length != expected || indent.Any(c => c != ' '))
        {
            line.AddError(nameof(Errors.CH0036), Errors.CH0036.Replace("{spaces}", expected.ToString()));
        }

        return true;
    }
}
