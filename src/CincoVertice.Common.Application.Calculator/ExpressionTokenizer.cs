using CincoVertice.Common.Application.Calculator.Models;
using System.Globalization;
using System.Text;

namespace CincoVertice.Common.Application.Calculator;

/// <summary>
///     Splits an expression into tokens.
///     <para>{Number}     ::= {Digit}+ ('.' {Digit}*)? | '.' {Digit}+</para>
///     <para>{Identifier} ::= ({Letter} | '_') ({Letter} | {Digit} | '_')*</para>
///     <para>{String}     ::= '"' ({Char} | '""')* '"'      // "" inside the text is one quote</para>
///     <para>{Symbol}     ::= [+-*/%^(),]</para>
///     <para>A '-' is always its own token; the parser decides whether it is a subtraction or a negation.</para>
/// </summary>
public class ExpressionTokenizer
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

    private void Advance()
    {
        _position++;
        _currentChar = _position < _expression.Length ? _expression[_position] : '\0';
    }
}
