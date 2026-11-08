namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CSkipLetter
{
    /// <summary>
    ///     Skips one letter character found from current CharIndex.
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>
    ///     If a letter character is found, return true.
    ///     Otherwise, return false and leave CharIndex in its original position.
    /// </returns>
    public static bool SkipLetter(this IGenericLexer lexer)
    {
        int startIndex = lexer.CharIndex;

        if (lexer.IsLetterChar())
        {
            lexer.NextChar();
        }

        return startIndex != lexer.CharIndex;
    }
}
