using CincoVertice.Common.Utils.Lexer.Extensions;

namespace CincoVertice.Common.Utils.Lexer.Tokens;

public static class CGetInteger
{
    /// <summary>
    ///     Gets Integer from CharIndex.
    ///     <para>Integer ::= '-'? {Digit}+ .</para>
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>GenericToken.
    ///     <para>If successful, Type = Integer.</para>
    ///     <para>Otherwise, Type = Null and returns CharIndex to initial position.</para>
    /// </returns>
    public static GenericToken GetInteger(this IGenericLexer lexer)
    {
        GenericToken token = lexer.NullToken();

        lexer.SkipChar('-');

        if (lexer.SkipDigits())
        {
            token.Length = lexer.CharIndex - token.StartPos;
            token.Type = GenericTokenType.Integer;
        }
        else
        {
            // No numbers found. Token already GenericTokenType.Null
            lexer.Char(token.StartPos);
        }

        return token;
    }
}
