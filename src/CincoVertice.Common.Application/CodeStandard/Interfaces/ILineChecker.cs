using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Interfaces;

public interface ILineChecker
{
    /// <summary>
    ///     Checks a line against a code standard rule.
    /// </summary>
    /// <param name="line">The line to evaluate.</param>
    /// <returns>
    ///     <see langword="true"/> to continue parsing; <see langword="false"/> to stop,
    ///     either due to an error or because further parsing is unnecessary.
    /// </returns>
    bool Check(LineModel line);
}
