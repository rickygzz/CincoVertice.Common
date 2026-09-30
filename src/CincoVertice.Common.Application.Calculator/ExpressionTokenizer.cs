using CincoVertice.Common.Application.Calculator.Models;
using System.Globalization;
using System.Text;

namespace CincoVertice.Common.Application.Calculator;

public class ExpressionTokenizer
{
    private string _expression = string.Empty;
    private int _position = 0;
    private char _currentChar = '\0';

    public List<Token> Tokenize(string? expression)
    {
        var tokens = new List<Token>();

        if (string.IsNullOrWhiteSpace(expression))
        {
            tokens.Add(new Token(TokenType.EndOfExpression, string.Empty, _position));
            return tokens;
        }

        _expression = expression;
        _position = 0;
        _currentChar = _expression[_position];

        while (_currentChar != '\0')
        {
            SkipWhitespace();

            if (char.IsDigit(_currentChar) || _currentChar == '.')
            {
                tokens.Add(ReadNumber());
                continue;
            }

            // Operators and parentheses
            Token? token = _currentChar switch
            {
                '+' => CreateToken(TokenType.Plus, "+"),
                '-' => CreateToken(TokenType.Minus, "-"),
                '*' => CreateToken(TokenType.Multiply, "*"),
                '/' => CreateToken(TokenType.Divide, "/"),
                '%' => CreateToken(TokenType.Modulo, "%"),
                '^' => CreateToken(TokenType.Power, "^"),
                '(' => CreateToken(TokenType.LeftParenthesis, "("),
                ')' => CreateToken(TokenType.RightParenthesis, ")"),
                _ => throw new InvalidOperationException($"Invalid character '{_currentChar}' at position {_position}")
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

        while (_currentChar != '\0' && (char.IsDigit(_currentChar) || _currentChar == '.'))
        {
            if (_currentChar == '.')
            {
                if (hasDecimalPoint)
                {
                    throw new InvalidOperationException($"Invalid number format: multiple decimal points at position {_position}");
                }
                hasDecimalPoint = true;
            }

            numberBuilder.Append(_currentChar);
            Advance();
        }

        string numberString = numberBuilder.ToString();

        // Validate the number
        if (!double.TryParse(numberString, NumberStyles.Float, CultureInfo.InvariantCulture, out _))
        {
            throw new InvalidOperationException($"Invalid number format '{numberString}' at position {startPosition}");
        }

        return new Token(TokenType.Number, numberString, startPosition);
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
