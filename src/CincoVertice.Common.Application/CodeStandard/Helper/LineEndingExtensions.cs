using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Helper;

public static class LineEndingExtensions
{
    /// <summary>
    ///     Returns the textual representation of a line ending.
    /// </summary>
    public static string ToText(this LineEndingEnum lineEnding)
    {
        return lineEnding switch
        {
            LineEndingEnum.CRLF => "\r\n",
            LineEndingEnum.LF => "\n",
            LineEndingEnum.CR => "\r",
            _ => string.Empty
        };
    }
}
