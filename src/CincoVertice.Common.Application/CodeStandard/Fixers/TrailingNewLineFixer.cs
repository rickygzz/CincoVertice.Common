using CincoVertice.Common.Application.CodeStandard.Interfaces;

namespace CincoVertice.Common.Application.CodeStandard.Fixers;

public sealed class TrailingNewLineFixer : IStringFixer
{
    public string Fix(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        string lineEnding = DetectLineEnding(content);
        string trimmed = content.TrimEnd('\r', '\n');

        return trimmed + lineEnding;
    }

    private static string DetectLineEnding(string content)
    {
        for (int i = 0; i < content.Length; i++)
        {
            if (content[i] == '\r')
            {
                bool followedByLf = i + 1 < content.Length && content[i + 1] == '\n';
                return followedByLf ? "\r\n" : "\r";
            }

            if (content[i] == '\n')
            {
                return "\n";
            }
        }

        return "\n";
    }
}
