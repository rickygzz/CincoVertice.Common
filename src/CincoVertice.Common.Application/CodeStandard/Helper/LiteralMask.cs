using System.Text;
using System.Text.RegularExpressions;

namespace CincoVertice.Common.Application.CodeStandard.Helper;

/// <summary>
///     Swaps every string and char literal for a one-line placeholder string, and back. Line-based fixers
///     (indentation, trailing whitespace, keyword spacing...) then can't change text inside literals, such as the
///     lines of a multi-line verbatim string or keyword test data in a string.
///     <para>The placeholder is itself a string literal, so code scanners still see a valid expression where the
///     literal was, and braces or parentheses inside literals no longer confuse them.</para>
/// </summary>
public sealed class LiteralMask
{
    private readonly List<string> _literals = [];

    // Makes the placeholders unique, so restoring cannot match text that was already in the code
    private readonly string _marker = "__lit_" + Guid.NewGuid().ToString("N")[..8] + "_";

    /// <summary>
    ///     Replaces each literal with a placeholder. Comments are left as they are.
    /// </summary>
    public string Mask(string content)
    {
        var sb = new StringBuilder(content.Length);
        int copied = 0;

        foreach (var (start, end) in CSharpLiteralScanner.FindLiterals(content))
        {
            sb.Append(content, copied, start - copied);
            sb.Append('"').Append(_marker).Append(_literals.Count).Append('"');
            _literals.Add(content[start..end]);
            copied = end;
        }

        sb.Append(content, copied, content.Length - copied);

        return sb.ToString();
    }

    /// <summary>
    ///     Puts the original literals back in place of their placeholders.
    /// </summary>
    public string Restore(string masked)
    {
        if (_literals.Count == 0)
        {
            return masked;
        }

        var placeholder = new Regex("\"" + Regex.Escape(_marker) + "([0-9]+)\"");

        return placeholder.Replace(masked, match => _literals[int.Parse(match.Groups[1].Value)]);
    }
}
