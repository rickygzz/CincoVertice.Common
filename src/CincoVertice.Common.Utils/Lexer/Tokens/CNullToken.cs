namespace CincoVertice.Common.Utils.Lexer.Tokens;

public static class CNullToken
{
    /// <summary>
    ///     Gets Null token with zero length, starting at lexer.CharIndex position.
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>Returns new GenericToken with pos=lexer.CharIndex, length=0 and type=Null.</returns>
    public static GenericToken NullToken(this IGenericLexer lexer)
    {
        return new GenericToken(lexer.CharIndex, 0, GenericTokenType.Null);
    }
}
