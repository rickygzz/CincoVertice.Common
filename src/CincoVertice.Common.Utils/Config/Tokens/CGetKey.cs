using CincoVertice.Common.Utils.Lexer;
using CincoVertice.Common.Utils.Lexer.Extensions;
using CincoVertice.Common.Utils.Lexer.Tokens;

namespace CincoVertice.Common.Utils.Config.Tokens;

public static class CGetKey
{
    /// <summary>
    /// Gets key from CharIndex.
    /// <para>{Key}     ::= ({Letter} | {Digit} | [_])+.</para>
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>GenericToken.
    ///     <para>If successfull, Type = Key.</para>
    ///     <para>Otherwise, Type = Null and returns CharIndex to initial position.</para>
    /// </returns>
    public static GenericToken GetKey(this ConfigLexer lexer)
    {
        GenericToken token = lexer.NullToken();

        while (lexer.IsLetterChar() || lexer.IsDigit() || lexer.CurrentChar == '_')
        {
            lexer.NextChar();
        }

        if (lexer.CharIndex > token.StartPos)
        {
            token.Length = lexer.CharIndex - token.StartPos;
            token.Type = ConfigTokenType.Key;
        }

        return token;
    }
}
