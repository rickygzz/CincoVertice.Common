using System.Text;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Fixers;

/// <summary>
///     Gives every line the same line ending: CRLF (Windows), LF (Linux, macOS) or CR (old Mac OS), whatever mix the
///     file has. Not part of the formatting pipelines: a fix runs it only when the user chooses a line ending.
/// </summary>
public sealed class LineEndingFixer : IStringFixer
{
    private readonly string _lineEnding;

    /// <param name="lineEnding">The line ending for every line; <see cref="LineEndingEnum.None"/> keeps them.</param>
    public LineEndingFixer(LineEndingEnum lineEnding = LineEndingEnum.LF)
    {
        _lineEnding = lineEnding.ToText();
    }

    /// <summary>
    ///     Converts contents' line endings: each CRLF, LF or CR becomes the one chosen.
    /// </summary>
    /// <param name="content">The content to convert.</param>
    /// <returns>Returns the converted string.</returns>
    public string Fix(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        if (_lineEnding.Length == 0)
        {
            return content;
        }

        var converted = new StringBuilder(content.Length);

        for (int i = 0; i < content.Length; i++)
        {
            char character = content[i];

            if (character == '\r')
            {
                // CRLF is one line ending, not two
                if (i + 1 < content.Length && content[i + 1] == '\n')
                {
                    i++;
                }

                converted.Append(_lineEnding);
            }
            else if (character == '\n')
            {
                converted.Append(_lineEnding);
            }
            else
            {
                converted.Append(character);
            }
        }

        return converted.ToString();
    }
}
