using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Interfaces;

public interface ILineCheckerService
{
    /// <summary>
    ///     Checks a line for code standard compliance. Errors are added to <see cref="LineModel.Errors"/>.
    /// </summary>
    /// <param name="line">
    ///     The line to check. Set by the caller: <see cref="LineModel.PreviousIndentationLevel"/>,
    ///     <see cref="LineModel.PreviousLineIsBlank"/> and <see cref="LineModel.PreviousCodeLine"/>.
    ///     Set by the check:
    ///     <see cref="LineModel.TrimmedContent"/>, <see cref="LineModel.IndentationLevel"/> and the checker state.
    /// </param>
    void CheckLine(LineModel line);
}
