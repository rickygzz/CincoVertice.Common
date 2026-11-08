namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CSkipDigits
{
    /// <summary>
    ///     Skips digits found from current CharIndex.
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>
    ///     If digits are found, return true.
    ///     Otherwise, return false and leave CharIndex in its original position.
    /// </returns>
    public static bool SkipDigits(this IGenericLexer lexer)
    {
        int startIndex = lexer.CharIndex;

        while (lexer.IsDigit())
        {
            lexer.NextChar();
        }

        return startIndex != lexer.CharIndex;
    }
}
