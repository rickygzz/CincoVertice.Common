using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Helper;

public static class LineParser
{
    public static List<LineModel> GetLines(string fileContent)
    {
        if (string.IsNullOrEmpty(fileContent))
        {
            return [];
        }

        var lines = new List<LineModel>();

        int lineNumber = 0;
        int start = 0;
        int i = 0;

        while (i < fileContent.Length)
        {
            char current = fileContent[i];

            if (current != '\r' && current != '\n')
            {
                i++;

                continue;
            }

            LineEndingEnum lineEnding;
            int lineEndingLength;

            if (current == '\r' && i + 1 < fileContent.Length && fileContent[i + 1] == '\n')
            {
                lineEnding = LineEndingEnum.CRLF;
                lineEndingLength = 2;
            }
            else if (current == '\n')
            {
                lineEnding = LineEndingEnum.LF;
                lineEndingLength = 1;
            }
            else
            {
                lineEnding = LineEndingEnum.CR;
                lineEndingLength = 1;
            }

            lines.Add(CreateLine(fileContent[start..i], ++lineNumber, lineEnding));

            i += lineEndingLength;
            start = i;
        }

        // A remaining segment means the last line has no trailing line ending.
        if (start < fileContent.Length)
        {
            lines.Add(CreateLine(fileContent[start..], ++lineNumber, LineEndingEnum.None));
        }

        return lines;
    }

    private static LineModel CreateLine(string content, int number, LineEndingEnum lineEnding)
    {
        return new LineModel
        {
            Number = number,
            Content = content,
            TrimmedContent = content.Trim(),
            LineEnding = lineEnding
        };
    }
}
