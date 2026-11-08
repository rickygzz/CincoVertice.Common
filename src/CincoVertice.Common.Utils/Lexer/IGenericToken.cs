namespace CincoVertice.Common.Utils.Lexer;

public interface IGenericToken
{
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
