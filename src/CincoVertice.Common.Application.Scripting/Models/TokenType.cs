namespace CincoVertice.Common.Application.Scripting.Models;

public enum TokenType
{
    Number,
    Text,
    Identifier,

    /// <summary>if, elseif, else, end, and, or, not, true, false. The value is lower case.</summary>
    Keyword,

    /// <summary>+ - * / % = == != &lt; &lt;= &gt; &gt;=</summary>
    Operator,

    LeftParenthesis,
    RightParenthesis,
    LeftBracket,
    RightBracket,
    Comma,

    /// <summary>End of a statement. Blank lines and new lines inside parentheses or brackets give none.</summary>
    NewLine,

    EndOfScript
}
