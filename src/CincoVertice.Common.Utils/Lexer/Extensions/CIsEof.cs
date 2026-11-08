namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CIsEOF
{
    /// <summary>
    ///     {Digit}      ::= [0123456789].
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>true if currentChar is {Digit}.</returns>
    public static bool IsEOF(this IGenericLexer lexer)
    {
        return lexer.CurrentChar == '\0';
    }
}
