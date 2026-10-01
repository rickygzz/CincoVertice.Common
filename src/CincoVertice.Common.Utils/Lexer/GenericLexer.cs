using CincoVertice.Common.Utils.Lexer.Extensions;
using CincoVertice.Common.Utils.Lexer.Models;

namespace CincoVertice.Common.Utils.Lexer;

public class GenericLexer : IGenericLexer
{
    /// <summary>
    ///     The external void Process() method to be called when Text property is changed.
    /// </summary>
    protected Process _processCallback;

    private string text = string.Empty;

    private readonly List<Line> lines = [];

    /// <summary>
    ///     Initializes a new instance of the <see cref="GenericLexer"/> class.
    /// </summary>
    public GenericLexer()
    {
        _processCallback = null!;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GenericLexer"/> class.
    ///     <para>There is an external Process() function: public void Process() {}.</para>
    /// </summary>
    /// <param name="processCallback">
    ///     External void Process() method to be called when Text property is changed. Null to be ignored.
    /// </param>
    public GenericLexer(Process processCallback)
    {
        _processCallback = processCallback;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="GenericLexer"/> class.
    ///     <para>No process function is used outside this class.</para>
    /// </summary>
    /// <param name="text">text to parse.</param>
    public GenericLexer(string text)
    {
        _processCallback = null!;

        Text = text;
    }

    /// <summary>External Process() function.</summary>
    public delegate void Process();

    /// <summary>
    /// List with indexes of all keys processed in Process().
    /// </summary>
    public GenericTokens Tokens = new();

    /// <summary>Gets character index for text[charIndex].</summary>
    public int CharIndex { get; private set; }

    /// <summary>Gets current char given by text[charIndex].</summary>
    public char CurrentChar { get; private set; }

    /// <summary>
    ///     Gets or sets this.text.
    ///     <para>Gets this.text.</para>
    ///     <para>Sets new CharIndex = 0 and CurrentChar pointing to CharIndex = 0.</para>
    ///     <para>If set, calls ProcessCallback.</para>
    /// </summary>
    public string Text
    {
        get
        {
            return text;
        }

        set
        {
            text = value;

            SetLinesList();

            Char(0);

            if (_processCallback != null)
            {
                _processCallback();
            }
        }
    }

    private void SetLinesList()
    {
        lines.Clear();

        Char(0);
        int charStart = 0;
        int charEnd = -1;
        while (CurrentChar != '\0')
        {
            if (this.IsEOL())
            {
                this.SkipOneEOL();

                charEnd = CharIndex - 1;

                lines.Add(new Line() { CharStart = charStart, CharEnd = charEnd });

                charStart = CharIndex;
            }
            NextChar();
        }

        if (CharIndex > charEnd)
        {
            lines.Add(new Line() { CharStart = charStart, CharEnd = CharIndex });
        }
    }

    /// <summary>
    ///     Gets this.text.Length.
    /// </summary>
    public int TextLength
    {
        get
        {
            return text.Length;
        }
    }

    /// <summary>
    ///     Retrieves a substring from this.text.
    /// </summary>
    /// <param name="startIndex">The zero-based starting character position.</param>
    /// <param name="length">The number of characters in the substring.</param>
    /// <returns>A substring. Returns string.Empty if length is zero.</returns>
    public string Substring(int startIndex, int length)
    {
        return text.Substring(startIndex, length);
    }

    /// <summary>
    /// Read next character from this.text and stores it in this.currentChar. Also increases this.charIndex.
    /// <para>If it reaches end of string, sets this.charIndex = text.Length and currentChar = '\0'.</para>
    /// </summary>
    public void NextChar()
    {
        Char(CharIndex + 1);
    }

    /// <summary>
    ///     Read previous character from this.text and stores it in this.currentChar. Also decreases this.charIndex.
    ///     <para>If it reaches start of string, does nothing.</para>
    /// </summary>
    public void PrevChar()
    {
        Char(CharIndex - 1);
    }

    /// <summary>
    ///     Sets charIndex and currentChar.
    /// </summary>
    /// <param name="index">
    ///     <para>Index to set CharIndex and CurrentChar.</para>
    ///     <para>
    ///         If index is outside this.text boundaries, CharIndex = this.text.Length and CurrentChar = '\0'.
    ///     </para>
    /// </param>
    public void Char(int index)
    {
        if (index >= 0 && index < text.Length)
        {
            CharIndex = index;

            CurrentChar = text[CharIndex];
        }
        else
        {
            // index is outside this.text boundaries.
            CharIndex = text.Length;

            CurrentChar = '\0';
        }
    }

    public Position Position(int charIndex)
    {
        for (int i = 0; i < lines.Count; i++)
        {
            if (charIndex <= lines[i].CharEnd)
            {
                return new Position
                {
                    Line = i + 1,
                    Column = charIndex - lines[i].CharStart + 1,
                };
            }
        }

        return new Position { Line = lines.Count, Column = lines[lines.Count - 1].CharEnd };
    }
}
