using CincoVertice.Common.Utils.Lexer.Extensions;

namespace CincoVertice.Common.Utils.Lexer.Tokens;

public static class CGetOct
{
    /// <summary>
    ///     Gets Oct from CharIndex.
    ///     <para>{OctDigit}   ::= [01234567].</para>
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>GenericToken.
    ///     <para>If successful, Type = Octal.</para>
    ///     <para>Otherwise, Type = Null and returns CharIndex to initial position.</para>
    /// </returns>
    public static GenericToken GetOct(this IGenericLexer lexer)
    {
        GenericToken token = lexer.NullToken();

        if (lexer.SkipOctDigits())
        {
            token.Length = lexer.CharIndex - token.StartPos;
            token.Type = GenericTokenType.Octal;
        }

        return token;
    }
}
