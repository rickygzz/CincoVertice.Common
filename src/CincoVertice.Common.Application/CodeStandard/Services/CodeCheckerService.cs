using System.Text.RegularExpressions;
using CincoVertice.Common.Application.CodeStandard.Checkers;
using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVerticeCommon.Application.CodeChecker;

public partial class CodeCheckerService : ICodeCheckerService
{
    public void CheckFile(string filePath, out List<ErrorModel> errors)
    {
        errors = [];

        if (!File.Exists(filePath))
        {
            errors.Add(Errors.New(0, nameof(Errors.CH0001), Errors.CH0001));

            return;
        }

        List<LineModel> lines = [];

        try
        {
            using StreamReader reader = new(filePath);

            lines = LineParser.GetLines(reader.ReadToEnd());
        }
        catch (Exception ex)
        {
            errors.Add(Errors.New(0, nameof(Errors.CH0002), Errors.CH0002.Replace("{message}", ex.Message)));

            return;
        }

        int previousIndentationLevel = 0;

        foreach (var line in lines)
        {
            line.PreviousIndentationLevel = previousIndentationLevel;

            CheckLine(line);

            // Blank lines never have their indentation computed, so they must not reset the
            // carried level; otherwise the next code line would falsely trip CH0014.
            if (!string.IsNullOrWhiteSpace(line.Content))
            {
                previousIndentationLevel = line.IndentationLevel;
            }

            errors.AddRange(line.Errors);
        }
    }

    /// <summary>
    ///     Checks line for code standard compliance.
    /// </summary>
    /// <param name="line">The line information.</param>
    /// <returns>Returns the indentation level.</returns>
    public static void CheckLine(LineModel line)
    {
        if (string.IsNullOrEmpty(line.Content))
        {
            return;
        }

        line.TrimmedContent = line.Content.Trim();

        if (string.IsNullOrEmpty(line.TrimmedContent))
        {
            line.AddError(nameof(Errors.CH0010), Errors.CH0010);

            return;
        }

        if (char.IsWhiteSpace(line.Content[^1]))
        {
            line.AddError(nameof(Errors.CH0011), Errors.CH0011);
        }

        if (line.Content.Length > 120)
        {
            line.AddError(nameof(Errors.CH0016), Errors.CH0016);
        }

        IndentationChecker indentationChecker = new();
        if (!indentationChecker.Check(line))
        {
            return;
        }

        if (!CheckDocumentationAndComments(line))
        {
            return;
        }

        CheckForbiddenStartTokens(line);

        CheckKeywordSpaceBeforeParenthesis(line);

        CheckForbiddenEndTokens(line);

        string[] forbiddenTokens = ["this."];
        foreach (var token in forbiddenTokens)
        {
            if (line.TrimmedContent.Contains(token))
            {
                line.AddError(nameof(Errors.CH0019), Errors.CH0019.Replace("{token}", token));
            }
        }
    }

    /// <summary>
    ///     Check line for documentation and comments.
    /// </summary>
    /// <param name="line">Line information.</param>
    /// <returns>Returns false to indicate it should stop parsing line for errors. Otherwise returns true.</returns>
    private static bool CheckDocumentationAndComments(LineModel line)
    {
        DocumentationChecker checker = new();

        if (checker.Check(line))
        {
            CommentChecker commentChecker = new();

            return commentChecker.Check(line);
        }

        return true;
    }

    private static void CheckForbiddenStartTokens(LineModel line)
    {
        string[] forbiddenStartTokens =
            [",", ";", ")", "(", "=", "+=", "-=", "*=", "/=", "%=", "==", "!=", ">=", "<=", "=>", "<", ">"];
        string pattern = @"\(\s*(\w+\s*(,\s*\w+\s*)*)?\)\s*([=>]|=)";

        foreach (var token in forbiddenStartTokens)
        {
            if (line.TrimmedContent.StartsWith(token))
            {
                if (token.Equals("(") && Regex.IsMatch(line.TrimmedContent, pattern))
                {
                    break;
                }

                line.AddError(nameof(Errors.CH0017), Errors.CH0017.Replace("{token}", token));
                break;
            }
        }
    }

    private static void CheckForbiddenEndTokens(LineModel line)
    {
        string[] forbiddenEndTokens =
            ["&&", "||", "??=", "??", "!", "?", ":", "+", "-", "/", "*"];

        foreach (var token in forbiddenEndTokens)
        {
            if (line.TrimmedContent.EndsWith(token))
            {
                if (token.Equals(":")
                    && (line.TrimmedContent.StartsWith("case")
                    || line.TrimmedContent.StartsWith("default")))
                {
                    break;
                }

                if (token.Equals("!") && !line.TrimmedContent.EndsWith(" !"))
                {
                    break;
                }

                line.AddError(nameof(Errors.CH0018), Errors.CH0018.Replace("{token}", token));

                break;
            }
        }
    }

    private static void CheckKeywordSpaceBeforeParenthesis(LineModel line)
    {
        if (line.TrimmedContent.Contains("new") && KeywordHasSpacesBeforeParenthesisRegex().IsMatch(line.TrimmedContent))
        {
            line.AddError(nameof(Errors.CH0023), Errors.CH0023.Replace("{keyword}", "new"));
        }

        if (line.TrimmedContent.StartsWith("for") && line.TrimmedContent.Length > 3)
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

                    return;
                }
                else if (KeywordHasMoreThanOneSpaceBeforeParenthesisRegex().IsMatch(line.TrimmedContent))
                {
                    line.AddError(nameof(Errors.CH0024), Errors.CH0024.Replace("{keyword}", "foreach"));

                    return;
                }
            }
            else if (KeywordHasMoreThanOneSpaceBeforeParenthesisRegex().IsMatch(line.TrimmedContent))
            {
                line.AddError(nameof(Errors.CH0024), Errors.CH0024.Replace("{keyword}", "for"));
            }

            return;
        }

        string[] keywords = ["if", "do", "while"];

        foreach (var keyword in keywords)
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
    }

    [GeneratedRegex(@"\b(?:for|foreach|do|while|if)\s{2,}\(")]
    private static partial Regex KeywordHasMoreThanOneSpaceBeforeParenthesisRegex();

    [GeneratedRegex(@"\bnew\s+\(")]
    private static partial Regex KeywordHasSpacesBeforeParenthesisRegex();
}
