using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Utils.Helpers;

namespace CincoVertice.Common.Application.CodeStandard.Fixers;

public sealed class LineEndingFixer : IStringFixer
{
    /// <summary>
    ///     Converts contents' line endings to LF.
    /// </summary>
    /// <param name="content">The content to convert.</param>
    /// <returns>Returns the converted string.</returns>
    public string Fix(string content)
    {
        string modified = StringLineEndingHelper.ToLF(content);

        return modified;
    }
}
