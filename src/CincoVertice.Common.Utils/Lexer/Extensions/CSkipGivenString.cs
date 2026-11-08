namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CSkipGivenString
{
    /// <summary>
    ///     Attempts to match and skip the specified string at the lexer's current character index.
    /// </summary>
    /// <param name="lexer">The lexer to advance when the string is matched.</param>
    /// <param name="str">The string to match.</param>
    /// <returns>
    ///     <see langword="true"/> if <paramref name="str"/> is matched and skipped; otherwise,
    ///     <see langword="false"/> and the lexer's character index is restored to its original position.
    /// </returns>
    public static bool SkipGivenString(this IGenericLexer lexer, string str)
    {
        int startIndex = lexer.CharIndex;
        for (int i = 0; i < str.Length; i++)
        {
            if (str[i] != lexer.CurrentChar)
            {
                lexer.Char(startIndex);
                return false;
            }

            lexer.NextChar();
        }

        return true;
    }
}
