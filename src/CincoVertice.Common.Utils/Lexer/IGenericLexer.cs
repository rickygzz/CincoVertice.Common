using CincoVertice.Common.Utils.Lexer.Models;

namespace CincoVertice.Common.Utils.Lexer;

/// <summary>
///     GenericLexer interface.
/// </summary>
public interface IGenericLexer
{
    /// <summary>Gets character index for text[charIndex].</summary>
    public int CharIndex { get; }

    /// <summary>Gets current char given by text[charIndex].</summary>
    public char CurrentChar { get; }

    /// <summary>
    ///     Gets or sets this.text.
    ///     <para>Gets this.text.</para>
    ///     <para>Sets new CharIndex = 0 and CurrentChar pointing to CharIndex = 0.</para>
    ///     <para>If set, calls ProcessCallback.</para>
    /// </summary>
    public string Text { get; set; }

    /// <summary>
    ///     Gets this.text.Length.
    /// </summary>
    public int TextLength { get; }

    /// <summary>
    ///     Retrieves a substring from this.text.
    /// </summary>
    /// <param name="startIndex">The zero-based starting character position.</param>
    /// <param name="length">The number of characters in the substring.</param>
    /// <returns>A substring. Returns string.Empty if length is zero.</returns>
    public string Substring(int startIndex, int length);

    /// <summary>
    ///     Read next character from this.text and stores it in this.currentChar. Also increases this.charIndex.
    ///     <para>If it reaches end of string, sets this.charIndex = text.Length and currentChar = '\0'.</para>
    /// </summary>
    public void NextChar();

    /// <summary>
    ///     Read previous character from this.text and stores it in this.currentChar. Also decreases this.charIndex.
    ///     <para>If it reaches start of string, does nothing.</para>
    /// </summary>
    public void PrevChar();

    /// <summary>
    ///     Sets charIndex and currentChar.
    /// </summary>
    /// <param name="index">Index to set CharIndex and CurrentChar.
    /// <para>If index is outside this.text boundaries, CharIndex = this.text.Length and CurrentChar = '\0'.</para></param>
    public void Char(int index);

    /// <summary>
    ///     Gets position for a given charIndex.
    /// </summary>
    /// <param name="charIndex">Character index.</param>
    /// <returns>Returns the position for the given charIndex.</returns>
    public Position Position(int charIndex);
}
