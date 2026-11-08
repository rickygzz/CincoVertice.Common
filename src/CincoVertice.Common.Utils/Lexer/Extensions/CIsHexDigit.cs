namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CIsHexDigit
{
    /// <summary>
    ///     {HexDigit}   ::= {Digit} + [abcdefABCDEF].
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>true if currentChar is {HexDigit}.</returns>
    public static bool IsHexDigit(this IGenericLexer lexer)
    {
        return lexer.IsDigit()
            || lexer.CurrentChar == 'A' || lexer.CurrentChar == 'a'
            || lexer.CurrentChar == 'B' || lexer.CurrentChar == 'b'
            || lexer.CurrentChar == 'C' || lexer.CurrentChar == 'c'
            || lexer.CurrentChar == 'D' || lexer.CurrentChar == 'd'
            || lexer.CurrentChar == 'E' || lexer.CurrentChar == 'e'
            || lexer.CurrentChar == 'F' || lexer.CurrentChar == 'f';
    }
}
