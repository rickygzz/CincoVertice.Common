using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Interfaces;

public interface ILineCheckerService
{
    /// <summary>
    ///     Checks a line for code standard compliance. Errors are added to <see cref="LineModel.Errors"/>.
    /// </summary>
    /// <param name="line">
    ///     The line to check. <see cref="LineModel.PreviousIndentationLevel"/> must be set by the caller;
    ///     <see cref="LineModel.TrimmedContent"/> and <see cref="LineModel.IndentationLevel"/> are set by the check.
    /// </param>
    void CheckLine(LineModel line);
}
