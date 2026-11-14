using CincoVertice.Common.Application.Calculator.Models;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace CincoVertice.Common.Application.Calculator;

/// <summary>
///     Splits an expression into tokens.
///     <para>{Number}     ::= {Digit}+ ('.' {Digit}*)? | '.' {Digit}+</para>
///     <para>{Time}       ::= {Digit}+ ':' {Digit}+</para>
///     <para>{Date}       ::= {Digit}{4} '-' {Digit}{1,2} '-' {Digit}{1,2}   // not 2026 minus 12 minus 25</para>
///     <para>{Identifier} ::= ({Letter} | '_') ({Letter} | {Digit} | '_')*</para>
///     <para>{String}     ::= '"' ({Char} | '""')* '"'      // "" inside the text is one quote</para>
///     <para>{Symbol}     ::= [+-*/%^(),=]</para>
///     <para>A '-' is always its own token; the parser decides whether it is a subtraction or a negation.</para>
/// </summary>
public partial class ExpressionTokenizer
{
    private string _expression = string.Empty;
    private int _position = 0;
    private char _currentChar = '\0';

    /// <summary>
    ///     Tokenizes the expression. The last token is always <see cref="TokenType.EndOfExpression"/>.
    /// </summary>
    /// <exception cref="ExpressionException">An invalid character or number was found.</exception>
    public List<Token> Tokenize(string? expression)
    {
        var tokens = new List<Token>();

        _expression = expression ?? string.Empty;
        _position = 0;
        _currentChar = _expression.Length > 0 ? _expression[0] : '\0';

        while (true)
        {
            SkipWhitespace();

            if (_currentChar == '\0')
            {
                break;
            }

            if (char.IsAsciiDigit(_currentChar) || _currentChar == '.')
            {
                tokens.Add(ReadNumber());
                continue;
            }

            if (char.IsAsciiLetter(_currentChar) || _currentChar == '_')
            {
                tokens.Add(ReadIdentifier());
                continue;
            }

            if (_currentChar == '"')
            {
                tokens.Add(ReadString());
                continue;
            }

            // Operators, parentheses and argument separator
            Token token = _currentChar switch
            {
                '+' => CreateToken(TokenType.Plus, "+"),
                '-' => CreateToken(TokenType.Minus, "-"),
                '*' => CreateToken(TokenType.Multiply, "*"),
                '/' => CreateToken(TokenType.Divide, "/"),
                '%' => CreateToken(TokenType.Modulo, "%"),
                '^' => CreateToken(TokenType.Power, "^"),
                '(' => CreateToken(TokenType.LeftParenthesis, "("),
                ')' => CreateToken(TokenType.RightParenthesis, ")"),
                ',' => CreateToken(TokenType.Comma, ","),
                '=' => CreateToken(TokenType.Assign, "="),
                _ => throw new ExpressionException($"Invalid character '{_currentChar}'", _position)
            };

            tokens.Add(token);
            Advance();
        }

        tokens.Add(new Token(TokenType.EndOfExpression, string.Empty, _position));

        return tokens;
    }

    private Token ReadNumber()
    {
        int startPosition = _position;
        var numberBuilder = new StringBuilder();
        bool hasDecimalPoint = false;

        while (char.IsAsciiDigit(_currentChar) || _currentChar == '.')
        {
            if (_currentChar == '.')
            {
                if (hasDecimalPoint)
                {
                    throw new ExpressionException("Invalid number format: multiple decimal points", _position);
                }

                hasDecimalPoint = true;
            }

            numberBuilder.Append(_currentChar);
            Advance();
        }

        string numberString = numberBuilder.ToString();

        // Four digits, then -month-day, is a date
        Match rest = DateRestRegex().Match(_expression, _position);

        if (!hasDecimalPoint && numberString.Length == 4 && rest.Success)
        {
            for (int i = 0; i < rest.Length; i++)
            {
                Advance();
            }

            return new Token(TokenType.Date, numberString + rest.Value, startPosition);
        }

        if (_currentChar == ':' && !hasDecimalPoint)
        {
            return ReadTime(numberString, startPosition);
        }

        if (numberString == ".")
        {
            throw new ExpressionException("Invalid number format '.'", startPosition);
        }

        // Only digits and one '.' remain, so a failure means it does not fit in a decimal
        if (!decimal.TryParse(numberString, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out _))
        {
            throw new ExpressionException($"Number is too large '{numberString}'", startPosition);
        }

        return new Token(TokenType.Number, numberString, startPosition);
    }

    /// <summary>
    ///     Reads the minutes of a time such as 10:30, after its hours. The parser checks the values.
    /// </summary>
    private Token ReadTime(string hours, int startPosition)
    {
        // Skip ':'
        Advance();

        int minutesPosition = _position;

        while (char.IsAsciiDigit(_currentChar))
        {
            Advance();
        }

        if (_position == minutesPosition)
        {
            throw new ExpressionException("Expected minutes after ':'", _position);
        }

        return new Token(TokenType.Time, $"{hours}:{_expression[minutesPosition.._position]}", startPosition);
    }

    private Token ReadIdentifier()
    {
        int startPosition = _position;

        while (char.IsAsciiLetterOrDigit(_currentChar) || _currentChar == '_')
        {
            Advance();
        }

        return new Token(TokenType.Identifier, _expression[startPosition.._position], startPosition);
    }

    /// <summary>
    ///     Reads text in double quotes. The token value is the text without the quotes, with "" turned into ".
    /// </summary>
    private Token ReadString()
    {
        int startPosition = _position;
        var textBuilder = new StringBuilder();

        // Skip opening quote
        Advance();

        while (true)
        {
            // Check the length, not '\0': the text itself may contain a '\0'
            if (_position >= _expression.Length)
            {
                throw new ExpressionException("Text is missing its closing quote", startPosition);
            }

            if (_currentChar == '"')
            {
                bool isEscapedQuote = _position + 1 < _expression.Length && _expression[_position + 1] == '"';

                if (!isEscapedQuote)
                {
                    // Skip closing quote
                    Advance();
                    break;
                }

                // Skip the first quote of ""; the second one is added below
                Advance();
            }

            textBuilder.Append(_currentChar);
            Advance();
        }

        return new Token(TokenType.String, textBuilder.ToString(), startPosition);
    }

    private Token CreateToken(TokenType type, string value)
    {
        return new Token(type, value, _position);
    }

    private void SkipWhitespace()
    {
        while (char.IsWhiteSpace(_currentChar))
        {
            Advance();
        }
    }

    // The -12-25 of 2026-12-25, right where the year ends; not a part of a longer number
    [GeneratedRegex(@"\G-\d{1,2}-\d{1,2}(?![\d.:])")]
    private static partial Regex DateRestRegex();

    private void Advance()
    {
        _position++;
        _currentChar = _position < _expression.Length ? _expression[_position] : '\0';
    }
}
