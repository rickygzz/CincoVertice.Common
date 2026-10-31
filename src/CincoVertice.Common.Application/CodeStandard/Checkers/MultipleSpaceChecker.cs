using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Checkers;

/// <summary>
///     CH0032: code must not contain consecutive spaces (private byte[] x  = ...). Indentation, literals and
///     comments are not checked, and the spaces before a comment are allowed, so comments can be aligned. Fixed by
///     MultipleSpaceFixer.
///     <para>Runs after the comment checker, which removes a trailing // comment from
///     <see cref="LineModel.TrimmedContent"/>, so only the code is checked.</para>
/// </summary>
public class MultipleSpaceChecker : ILineChecker
{
    private static readonly string[] _keywords = ["if", "for", "foreach", "while", "do", "new"];
    /// <inheritdoc/>
    public bool Check(LineModel line)
    {
        string code = line.TrimmedContent;

        // Lines of a multi-line string or of a /* */ comment
        if (line.StartsInsideLiteral || code.StartsWith("/*") || code.StartsWith('*'))
        {
            return true;
        }

        int i = 0;

        while (i < code.Length)
        {
            char current = code[i];

            if (current is '"' or '\'' or '$' or '@')
            {
                i = CSharpLiteralScanner.SkipLiteral(code, i);
            }
            else if (current == '/' && i + 1 < code.Length && code[i + 1] == '*')
            {
                int end = code.IndexOf("*/", i + 2, StringComparison.Ordinal);

                if (end < 0)
                {
                    return true;
                }

                i = end + 2;
            }
            else if (current == ' ' && i + 1 < code.Length && code[i + 1] == ' ')
            {
                int run = i;

                while (run < code.Length && code[run] == ' ')
                {
                    run++;
                }

                if (!code[run..].StartsWith("/*") && !IsKeywordBeforeParenthesis(code, i, run))
                {
                    line.AddError(nameof(Errors.CH0032), Errors.CH0032);

                    return true;
                }

                i = run;
            }
            else
            {
                i++;
            }
        }

        return true;
    }

    /// <summary>
    ///     True for the spaces between a keyword (if, new...) and its parenthesis, which
    ///     KeywordParenthesisSpaceChecker already reports (CH0023, CH0024).
    /// </summary>
    private static bool IsKeywordBeforeParenthesis(string code, int runStart, int runEnd)
    {
        if (runEnd >= code.Length || code[runEnd] != '(')
        {
            return false;
        }

        int wordStart = runStart;

        while (wordStart > 0 && char.IsLetter(code[wordStart - 1]))
        {
            wordStart--;
        }

        return _keywords.Contains(code[wordStart..runStart]);
    }
}
