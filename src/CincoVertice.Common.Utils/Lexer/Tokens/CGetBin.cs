using CincoVertice.Common.Utils.Lexer.Extensions;

namespace CincoVertice.Common.Utils.Lexer.Tokens;

public static class CGetBin
{
    /// <summary>
    ///     Gets bin from CharIndex.
    ///     <para>{BinDigit}   ::= [01].</para>
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>GenericToken.
    ///     <para>If successful, Type = Bin.</para>
    ///     <para>Otherwise, Type = Null and returns CharIndex to initial position.</para>
    /// </returns>
    public static GenericToken GetBin(this IGenericLexer lexer)
    {
        GenericToken token = lexer.NullToken();

        if (lexer.SkipBinDigits())
        {
            token.Length = lexer.CharIndex - token.StartPos;
            token.Type = GenericTokenType.Binary;
        }

        return token;
    }
}
