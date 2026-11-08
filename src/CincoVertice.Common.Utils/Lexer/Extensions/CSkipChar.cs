namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CSkipChar
{
    /// <summary>
    ///     Skips character if found from current CharIndex.
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <param name="c">Character to skip.</param>
    /// <returns>
    ///     If c is found, return true.
    ///     Otherwise, return false and leave CharIndex in its original position.
    /// </returns>
    public static bool SkipChar(this IGenericLexer lexer, char c)
    {
        if (lexer.CurrentChar == c)
        {
            lexer.NextChar();

            return true;
        }

        return false;
    }
}
