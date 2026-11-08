using System.Text;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Fixers.CSharp;

/// <summary>
///     Fixes CH0017 (see ForbiddenStartTokenChecker): moves a separator or operator at the start of a line (",", ";",
///     ")", "=", "==", "=>"...) to the end of the previous code line, before its trailing comment. A line left empty
///     is removed, so "Foo(\n    a\n);" becomes "Foo(\n    a);".
///     <para>A line starting with "(" is left as it is, because it usually starts a new expression. Nothing is moved
///     after a line ending in ";" or "{", or after a preprocessor line, because the token does not continue it.</para>
/// </summary>
public sealed class ForbiddenStartTokenFixer : IStringFixer
{
    // Longest first, so ">>=" wins over ">>" over ">"
    private static readonly string[] _tokens =
    [
        ">>>=", "<<=", ">>=", ">>>",
        "+=", "-=", "*=", "/=", "%=", "&=", "|=", "^=", "==", "!=", ">=", "<=", "=>", "<<", ">>",
        ",", ";", ")", "=", "<", ">"
    ];

    // Separators join the previous line directly; operators get a space before them
    private static readonly string[] _separators = [",", ";", ")"];

    public string Fix(string content)
    {
        if (string.IsNullOrEmpty(content))
        {
            return string.Empty;
        }

        var lines = LineParser.GetLines(content);
        var spans = LineSpans.Find(content, lines);
        var result = new List<LineModel>(lines.Count);
        LineModel? previousCode = null;

        for (int i = 0; i < lines.Count; i++)
        {
            var line = lines[i];

            if (spans[i].StartsInside)
            {
                result.Add(line);

                continue;
            }

            bool moved = false;
            string code = line.Content.TrimStart();

            while (previousCode is not null && CanContinue(previousCode.Content) && TakeToken(code) is { } token)
            {
                previousCode.Content = AppendToCode(previousCode.Content, token);
                code = code[token.Length..].TrimStart();
                moved = true;
            }

            if (moved)
            {
                if (code.Length == 0)
                {
                    continue;
                }

                line.Content = IndentHelper.GetIndent(line.Content) + code;
            }

            result.Add(line);

            if (code.Length > 0 && !code.StartsWith("//"))
            {
                previousCode = line;
            }
        }

        var sb = new StringBuilder(content.Length);

        foreach (var line in result)
        {
            sb.Append(line.Content).Append(line.LineEnding.ToText());
        }

        return sb.ToString();
    }

    private static string? TakeToken(string code)
    {
        foreach (string token in _tokens)
        {
            if (code.StartsWith(token, StringComparison.Ordinal))
            {
                return token;
            }
        }

        return null;
    }

    /// <summary>
    ///     False when the line ends a statement or opens a block (; or {), or is a preprocessor line.
    /// </summary>
    private static bool CanContinue(string lineContent)
    {
        string code = lineContent[..LineSpans.CodeEnd(lineContent)].Trim();

        return code.Length > 0 && !code.StartsWith('#') && code[^1] is not (';' or '{');
    }

    /// <summary>
    ///     Appends the token after the code of the line, keeping a trailing comment after it.
    /// </summary>
    private static string AppendToCode(string lineContent, string token)
    {
        string code = lineContent[..LineSpans.CodeEnd(lineContent)].TrimEnd();
        string separator = _separators.Contains(token) ? string.Empty : " ";

        return code + separator + token + lineContent[code.Length..];
    }
}
