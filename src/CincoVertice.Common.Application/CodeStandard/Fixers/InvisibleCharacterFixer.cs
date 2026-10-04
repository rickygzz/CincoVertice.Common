using System.Text;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;

namespace CincoVertice.Common.Application.CodeStandard.Fixers;

/// <summary>
///     Replaces Unicode spaces with a normal space and removes invisible characters (see
///     <see cref="UnicodeCharacters"/>). Visible characters, including accented letters, are kept.
/// </summary>
public sealed class InvisibleCharacterFixer : IStringFixer
{
    public string Fix(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        var sb = new StringBuilder(content.Length);

        for (int i = 0; i < content.Length; i++)
        {
            char c = content[i];

            // A byte order mark is only valid at the start of the file
            if (c == '\uFEFF' && i == 0)
            {
                sb.Append(c);
            }
            else if (UnicodeCharacters.IsInvisible(c))
            {
                continue;
            }
            else if (UnicodeCharacters.IsUnicodeSpace(c))
            {
                sb.Append(' ');
            }
            else
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }
}
