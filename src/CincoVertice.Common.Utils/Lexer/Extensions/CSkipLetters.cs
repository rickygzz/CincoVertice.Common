namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CSkipLetters
{
    /// <summary>
    ///     Skips letter characters found from current CharIndex.
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>
    ///     If letter characters are found, return true.
    ///     Otherwise, return false and leave CharIndex in its original position.
    /// </returns>
    public static bool SkipLetters(this IGenericLexer lexer)
    {
        int startIndex = lexer.CharIndex;

        while (lexer.IsLetterChar())
        {
            lexer.NextChar();
        }

        return startIndex != lexer.CharIndex;
    }
}
