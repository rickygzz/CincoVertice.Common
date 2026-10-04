using System.Text.RegularExpressions;
using CincoVertice.Common.Application.CodeStandard.Interfaces;

namespace CincoVertice.Common.Application.CodeStandard.Fixers;

public sealed partial class KeywordParenthesisSpaceFixer : IStringFixer
{
    public string Fix(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        // 1) Normalize the gap between a control-flow keyword and its '(' to exactly one space.
        string normalized = KeywordBeforeParenthesisRegex().Replace(content, "$1 (");

        // 2) A closing brace at the start of a line followed by a control-flow statement:
        //    keep 'while' on the same line (do-while), move anything else onto its own line
        //    at the brace's indentation.
        string bracesSplit = ClosingBraceBeforeStatementRegex().Replace(
            normalized,
            match =>
            {
                string indent = match.Groups[1].Value;
                string statement = match.Groups[2].Value;

                return statement.StartsWith("while", StringComparison.Ordinal)
                    ? $"{indent}}} {statement}"
                    : $"{indent}}}\n{indent}{statement}";
            });

        // 3) A control-flow statement whose line ends with an opening brace: move the brace onto
        //    its own line at the same indentation as the statement.
        return OpenBraceAtLineEndRegex().Replace(bracesSplit, "${1}${2}\n${1}{");
    }

    // A control-flow keyword followed by any run of spaces/tabs (including none) and an opening
    // parenthesis on the same line, so it can be normalized to exactly one space.
    [GeneratedRegex(@"\b(if|foreach|for|while|do)[ \t]*\(")]
    private static partial Regex KeywordBeforeParenthesisRegex();

    // A line that starts (after optional indentation) with a closing brace, then a control-flow
    // statement: 'if/for/foreach/while', optionally preceded by 'else'. The rest of the statement
    // is captured up to (but not including) the line ending.
    [GeneratedRegex(@"(?m)^([ \t]*)\}[ \t]*((?:else[ \t]+)?(?:if|foreach|for|while)\b[^\n]*)")]
    private static partial Regex ClosingBraceBeforeStatementRegex();

    // A control-flow statement line that ends with an opening brace (ignoring trailing spaces).
    // The lookahead matches the line ending without consuming it, so CRLF/LF are preserved.
    [GeneratedRegex(@"(?m)^([ \t]*)((?:else[ \t]+)?(?:if|foreach|for|while|do)\b[^\n]*?)[ \t]*\{[ \t]*(?=\n|$)")]
    private static partial Regex OpenBraceAtLineEndRegex();
}
