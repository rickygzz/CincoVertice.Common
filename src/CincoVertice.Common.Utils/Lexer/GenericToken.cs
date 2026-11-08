namespace CincoVertice.Common.Utils.Lexer;

public class GenericToken : IGenericToken
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GenericToken"/> class.
    /// </summary>
    /// <param name="startPos">StartPos.</param>
    /// <param name="length">Length.</param>
    /// <param name="type">Token type.</param>
    public GenericToken(int startPos = -1, int length = 0, int type = 0)
    {
        StartPos = startPos;
        Length = length;
        Type = type > 0 ? type : GenericTokenType.Null;
    }

    /// <summary>
    ///     Gets or sets Token start position.
    /// </summary>
    public int StartPos { get; set; }

    /// <summary>
    ///     Gets or sets Token length.
    /// </summary>
    public int Length { get; set; }

    /// <summary>
    ///     Gets or sets Token type.
    /// </summary>
    public int Type { get; set; }
}
