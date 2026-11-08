using System.Text.RegularExpressions;
using CincoVertice.Common.Application.CodeStandard.Interfaces;

namespace CincoVertice.Common.Application.CodeStandard.Fixers.CSharp;

public sealed partial class KeywordParenthesisSpaceFixer : IStringFixer
{
    public string Fix(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        // Normalize the gap between a control-flow keyword and its '(' to exactly one space. Brace placement is
        // BracePlacementFixer's job.
        return KeywordBeforeParenthesisRegex().Replace(content, "$1 (");
    }

    // A control-flow keyword followed by any run of spaces/tabs (including none) and an opening
    // parenthesis on the same line, so it can be normalized to exactly one space.
    [GeneratedRegex(@"\b(if|foreach|for|while|do)[ \t]*\(")]
    private static partial Regex KeywordBeforeParenthesisRegex();
}
