namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CIsBinDigit
{
    /// <summary>
    /// {BinDigit}   ::= [01].
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>true if currentChar is {BinDigit}.</returns>
    public static bool IsBinDigit(this IGenericLexer lexer)
    {
        return lexer.CurrentChar == '0' || lexer.CurrentChar == '1';
    }
}
