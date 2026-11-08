namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CIsAlphanumericChar
{
    /// <summary>
    /// {Alphanumeric] ::= {Letter} + {Digit}.
    /// <para>{Letter}    ::= [A-Za-z] [0xC0-0xD6] [0xD8-0xF6] [0xF8-0x2FF].</para>
    /// <para>{Digit}     ::= [0123456789].</para>
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>true if currentChar is {Alphanumeric}.</returns>
    public static bool IsAlphanumericChar(this IGenericLexer lexer)
    {
        return lexer.IsLetterChar() || lexer.IsDigit();
    }
}
