namespace CincoVertice.Common.Application.Calculator.Models;

public enum TokenType
{
    Number,
    Time,

    /// <summary>A date as year-month-day: 2026-12-25.</summary>
    Date,
    String,
    Identifier,
    Plus,
    Minus,
    Multiply,
    Divide,
    Modulo,
    Power,
    LeftParenthesis,
    RightParenthesis,
    Comma,

    /// <summary>The '=' that gives a value a name: x = 5.</summary>
    Assign,
    EndOfExpression
}
