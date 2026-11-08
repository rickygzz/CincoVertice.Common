namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CSkipHexDigits
{
    /// <summary>
    ///     Skips hex digits found from current CharIndex.
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>
    ///     If hex digits are found, return true.
    ///     Otherwise, return false and leave CharIndex in its original position.
    /// </returns>
    public static bool SkipHexDigits(this IGenericLexer lexer)
    {
        int startIndex = lexer.CharIndex;

        while (lexer.IsHexDigit())
        {
            lexer.NextChar();
        }

        return startIndex != lexer.CharIndex;
    }
}
