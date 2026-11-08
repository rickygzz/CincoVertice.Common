namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CSkipBinDigits
{
    /// <summary>
    ///     Skips bin digits found from current CharIndex.
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>
    ///     If bin digits are found, return true.
    ///     Otherwise, return false and leave CharIndex in its original position.
    /// </returns>
    public static bool SkipBinDigits(this IGenericLexer lexer)
    {
        int startIndex = lexer.CharIndex;

        while (lexer.IsBinDigit())
        {
            lexer.NextChar();
        }

        return startIndex != lexer.CharIndex;
    }
}
