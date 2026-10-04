using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.Tests.CodeChecker.Checkers.TestHelpers;

/// <summary>
///     Builds a <see cref="LineModel"/> as <c>LineCheckerService</c> passes it to the checkers,
///     with <see cref="LineModel.TrimmedContent"/> already set.
/// </summary>
internal static class TestLine
{
    public static LineModel Create(string content, int number = 1, bool previousLineIsBlank = false)
    {
        return new LineModel
        {
            Content = content,
            TrimmedContent = content.Trim(),
            Number = number,
            PreviousLineIsBlank = previousLineIsBlank
        };
    }
}
