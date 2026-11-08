namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CIsOctDigit
{
    /// <summary>
    ///     {OctDigit}   ::= [01234567].
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>true if currentChar is {OctDigit}.</returns>
    public static bool IsOctDigit(this IGenericLexer lexer)
    {
        return lexer.CurrentChar == '0' || lexer.CurrentChar == '1'
            || lexer.CurrentChar == '2' || lexer.CurrentChar == '3'
            || lexer.CurrentChar == '4' || lexer.CurrentChar == '5'
            || lexer.CurrentChar == '6' || lexer.CurrentChar == '7';
    }
}
