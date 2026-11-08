using System.Text;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;

namespace CincoVertice.Common.Application.CodeStandard.Fixers;

/// <summary>
///     Replaces typographic punctuation with ASCII in code and comments: curly quotes, dashes, the minus sign and
///     the ellipsis, which are typical of AI-written text. In code they do not compile, e.g. curly quotes around
///     a string or a minus sign in a - b. String and char literals are never changed, since there the characters
///     may be intentional (e.g. text shown to users).
///     <para>Limitation: quotes inside an interpolation hole, as in $"{a["x"]}", end the string early. The
///     scanner resynchronizes at the next quote, so at worst some code after it is left unchanged.</para>
/// </summary>
public sealed class TypographicCharacterFixer : IStringFixer
{
    private static readonly Dictionary<char, string> _replacements = new()
    {
        // Single quotes and prime
        ['\u2018'] = "'",
        ['\u2019'] = "'",
        ['\u201A'] = "'",
        ['\u201B'] = "'",
        ['\u2032'] = "'",

        // Double quotes and double prime
        ['\u201C'] = "\"",
        ['\u201D'] = "\"",
        ['\u201E'] = "\"",
        ['\u201F'] = "\"",
        ['\u2033'] = "\"",

        // Hyphens, dashes and minus sign
        ['\u2010'] = "-",
        ['\u2011'] = "-",
        ['\u2012'] = "-",
        ['\u2013'] = "-",
        ['\u2014'] = "-",
        ['\u2015'] = "-",
        ['\u2212'] = "-",

        // Ellipsis
        ['\u2026'] = "...",
    };

    public string Fix(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        var sb = new StringBuilder(content.Length);
        int copied = 0;

        // Comments are replaced as a whole; the code between them is scanned for literals
        foreach (var comment in CSharpLiteralScanner.FindComments(content))
        {
            AppendCode(sb, content, copied, comment.Start);
            AppendReplaced(sb, content, comment.Start, comment.End);
            copied = comment.End;
        }

        AppendCode(sb, content, copied, content.Length);

        return sb.ToString();
    }

    /// <summary>
    ///     Appends code, copying string and char literals unchanged and replacing everything else.
    /// </summary>
    private static void AppendCode(StringBuilder sb, string content, int start, int end)
    {
        int i = start;

        while (i < end)
        {
            if (content[i] is '"' or '\'' or '$' or '@')
            {
                // Not a literal ($ or @ alone) returns i + 1, so the character is copied as is
                int literalEnd = Math.Min(CSharpLiteralScanner.SkipLiteral(content, i), end);

                sb.Append(content, i, literalEnd - i);
                i = literalEnd;

                continue;
            }

            AppendReplaced(sb, content, i, i + 1);
            i++;
        }
    }

    private static void AppendReplaced(StringBuilder sb, string content, int start, int end)
    {
        for (int i = start; i < end; i++)
        {
            if (_replacements.TryGetValue(content[i], out string? replacement))
            {
                sb.Append(replacement);
            }
            else
            {
                sb.Append(content[i]);
            }
        }
    }
}
