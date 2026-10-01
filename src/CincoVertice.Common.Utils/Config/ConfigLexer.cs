using System.Text;
using CincoVertice.Common.Utils.Config.Tokens;
using CincoVertice.Common.Utils.Lexer;
using CincoVertice.Common.Utils.Lexer.Extensions;

namespace CincoVertice.Common.Utils.Config;

public class ConfigLexer : GenericLexer
{
    /// <summary>
    ///     List with indexes of all keys processed in Process().
    /// </summary>
    public ConfigItems Items;

    public ConfigLexer()
        : base()
    {
        Items = new ConfigItems(this);

        // Create new lexer with Process callback function.ç
        _processCallback = Process;
    }

    public ConfigLexer(string text)
        : base()
    {
        Items = new ConfigItems(this);

        // Create new lexer with Process callback function.ç
        _processCallback = Process;
        Text = text;
    }

    /// <summary>
    ///     Processes the input text and generates tokens.
    /// </summary>
    private new void Process()
    {
        Tokens.Clear();
        Items.Clear();

        Char(0);

        while (NextToken())
        {
            // Continue until no more tokens are found (NextToken() returns false.)
        }
    }

    /// <summary>
    ///     Retrieves the next token from the input text.
    /// </summary>
    /// <returns>true if a valid token was found, otherwise false.</returns>
    private bool NextToken()
    {
        // {Token}     ::=  {Space}* ({Key-Value} | {Comment}) {EOL}
        // {Key-Value} ::=  {Key} {Space}* [=] {Space}* ({Value} | {String})
        // {Comment}   ::=  [#] {Printable}
        this.SkipBlankChars();

        GenericToken token = this.GetComment();
        if (token.Type == ConfigTokenType.Comment)
        {
            Tokens.Add(token);
            return true;
        }

        ConfigItemModel item = this.GetKeyValueTokens();
        if (item.Key.Type == ConfigTokenType.Key)
        {
            Tokens.Add(item.Key);
            Tokens.Add(item.Value);

            Items.AddKeyValue(item);

            return true;
        }

        // Error: Unknown token.
        token.Type = ConfigTokenType.ErrorUnknown;

        this.SkipCharsToNextLineOrEOF();

        if (CurrentChar == '\0')
        {
            // End Of File. Exit NextToken loop.
            return false;
        }

        // Look for next token in next line.
        return true;
    }

    public string TokensToText(int fromIndex = 0, int toIndex = -1)
    {
        if (fromIndex >= Tokens.Count)
        {
            return string.Empty;
        }

        if (fromIndex < 0)
        {
            fromIndex = 0;
        }

        if (toIndex == -1 || toIndex >= Tokens.Count)
        {
            toIndex = Tokens.Count - 1;
        }

        StringBuilder sb = new(TextLength);

        for (int i = fromIndex; i < toIndex; i++)
        {
            GenericToken token = Tokens.Token(i);
            if (token.Type == ConfigTokenType.Comment)
            {
                sb.Append(Substring(token.StartPos, token.Length));
            }
            else if (token.Type == ConfigTokenType.Key)
            {
                sb.Append(Substring(token.StartPos, token.Length) + " = ");

                i++;

                if (i <= toIndex)
                {
                    token = Tokens.Token(i);
                    if (token.Type == ConfigTokenType.StringQuoted)
                    {
                        // Append <String_Quoted>
                        sb.Append("[\"" + Substring(token.StartPos, token.Length) + "\"]");
                    }
                    else if (token.Type < ConfigTokenType.ErrorUnknown)
                    {
                        // Append values
                        sb.Append(Substring(token.StartPos, token.Length));
                    }
                }
            }

            if (i < toIndex - 1)
            {
                sb.Append(Environment.NewLine);
            }
        }

        return sb.ToString();
    }
}
