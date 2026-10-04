using System.Text;
using CincoVertice.Common.Application.CodeStandard.Interfaces;

namespace CincoVertice.Common.Application.CodeStandard.Fixers;

/// <summary>
///     Fixes CH0010 and CH0011 (see TrailingWhitespaceChecker): removes whitespace at the end of every line,
///     which leaves a whitespace-only line empty.
/// </summary>
public sealed class TrailingWhitespaceFixer : IStringFixer
{
    public string Fix(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        var sb = new StringBuilder(content.Length);
        var line = new StringBuilder();

        int i = 0;
        while (i < content.Length)
        {
            char c = content[i];

            if (c == '\r' || c == '\n')
            {
                int end = line.Length;
                while (end > 0 && char.IsWhiteSpace(line[end - 1]))
                {
                    end--;
                }

                sb.Append(line.ToString(0, end));
                sb.Append(c);

                if (i + 1 < content.Length)
                {
                    char next = content[i + 1];
                    if ((c == '\r' && next == '\n') || (c == '\n' && next == '\r'))
                    {
                        sb.Append(next);
                        i++;
                    }
                }

                line.Clear();
            }
            else
            {
                line.Append(c);
            }

            i++;
        }

        int lastEnd = line.Length;
        while (lastEnd > 0 && char.IsWhiteSpace(line[lastEnd - 1]))
        {
            lastEnd--;
        }

        sb.Append(line.ToString(0, lastEnd));

        return sb.ToString();
    }
}
