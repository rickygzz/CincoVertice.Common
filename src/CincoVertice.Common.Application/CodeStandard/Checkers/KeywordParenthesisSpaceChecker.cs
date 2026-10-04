using System.Text.RegularExpressions;
using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Checkers;

/// <summary>
///     Space between a keyword and its parenthesis:
///     <para>CH0022: for, foreach, if, do and while need one space before "(".</para>
///     <para>CH0023: new must not have a space before "(".</para>
///     <para>CH0024: only one space is allowed.</para>
/// </summary>
public partial class KeywordParenthesisSpaceChecker : ILineChecker
{
    private static readonly string[] _keywords = ["if", "do", "while"];

    public bool Check(LineModel line)
    {
        if (line.TrimmedContent.Contains("new")
            && KeywordHasSpacesBeforeParenthesisRegex().IsMatch(line.TrimmedContent))
        {
            line.AddError(nameof(Errors.CH0023), Errors.CH0023.Replace("{keyword}", "new"));
        }

        if (line.TrimmedContent.StartsWith("for") && line.TrimmedContent.Length > 3)
        {
            CheckForAndForeach(line);

            return true;
        }

        foreach (var keyword in _keywords)
        {
            if (line.TrimmedContent.StartsWith(keyword)
                && line.TrimmedContent.Length > keyword.Length)
            {
                if (line.TrimmedContent[keyword.Length] == '(')
                {
                    line.AddError(nameof(Errors.CH0022), Errors.CH0022.Replace("{keyword}", keyword));
                }
                else if (KeywordHasMoreThanOneSpaceBeforeParenthesisRegex().IsMatch(line.TrimmedContent))
                {
                    line.AddError(nameof(Errors.CH0024), Errors.CH0024.Replace("{keyword}", keyword));
                }

                break;
            }
        }

        return true;
    }

    private static void CheckForAndForeach(LineModel line)
    {
        if (line.TrimmedContent[3] == '(')
        {
            line.AddError(nameof(Errors.CH0022), Errors.CH0022.Replace("{keyword}", "for"));
        }
        else if (line.TrimmedContent.StartsWith("foreach"))
        {
            if (line.TrimmedContent.Length > 7 && line.TrimmedContent[7] == '(')
            {
                line.AddError(nameof(Errors.CH0022), Errors.CH0022.Replace("{keyword}", "foreach"));
            }
            else if (KeywordHasMoreThanOneSpaceBeforeParenthesisRegex().IsMatch(line.TrimmedContent))
            {
                line.AddError(nameof(Errors.CH0024), Errors.CH0024.Replace("{keyword}", "foreach"));
            }
        }
        else if (KeywordHasMoreThanOneSpaceBeforeParenthesisRegex().IsMatch(line.TrimmedContent))
        {
            line.AddError(nameof(Errors.CH0024), Errors.CH0024.Replace("{keyword}", "for"));
        }
    }

    [GeneratedRegex(@"\b(?:for|foreach|do|while|if)\s{2,}\(")]
    private static partial Regex KeywordHasMoreThanOneSpaceBeforeParenthesisRegex();

    [GeneratedRegex(@"\bnew\s+\(")]
    private static partial Regex KeywordHasSpacesBeforeParenthesisRegex();
}
