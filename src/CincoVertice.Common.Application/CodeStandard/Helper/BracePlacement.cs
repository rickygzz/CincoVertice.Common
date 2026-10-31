namespace CincoVertice.Common.Application.CodeStandard.Helper;

/// <summary>
///     Block brace placement shared by the brace placement checker and fixer (CH0034), so both agree on which
///     braces must go on their own line. Both take the code of a trimmed line, without its trailing comment.
/// </summary>
public static class BracePlacement
{
    /// <summary>
    ///     True for a line ending with the { that opens a block after code: "if (x) {", "void Run() {", "x => {".
    /// </summary>
    public static bool OpeningBraceHasCodeBefore(string code) => code.Length > 1 && code[^1] == '{';

    /// <summary>
    ///     True for a } followed by a statement on the same line: "} else", "} catch (e)", "} if (x)". A } followed
    ///     by punctuation ("});", "},", "}.Run()") or by the while of a do-while stays.
    /// </summary>
    public static bool ClosingBraceHasCodeAfter(string code)
    {
        if (!code.StartsWith('}'))
        {
            return false;
        }

        string rest = code[1..].TrimStart();

        return rest.Length > 0 && (char.IsLetter(rest[0]) || rest[0] is '_' or '@') && !StartsWithWord(rest, "while");
    }

    private static bool StartsWithWord(string text, string word) =>
        text.StartsWith(word, StringComparison.Ordinal)
        && (text.Length == word.Length || !char.IsLetterOrDigit(text[word.Length]) && text[word.Length] != '_');
}
