using CincoVertice.Common.Application.Calculator.Ast;
using CincoVertice.Common.Application.Calculator.Models;
using System.Globalization;
using System.Text;

namespace CincoVertice.Common.Application.Calculator;

/// <summary>
///     Builds a syntax tree from the tokens returned by <see cref="ExpressionTokenizer"/> (recursive descent).
///     <para>{Calculation} ::= {TimeConversion} | {Expression} ({Unit} 'to' {Unit})?</para>
///     <para>{Unit}       ::= {Identifier} ('/' {Identifier} | '^' {Number})?</para>
///     <para>{TimeConversion} ::= ({Time} ('am' | 'pm')? | {Number} ('am' | 'pm') | 'now') {Zone}? 'to' {Zone}</para>
///     <para>{Zone}       ::= {Identifier} ('/' {Identifier} | {Identifier})* (('+' | '-') ({Number} | {Time}))?
///                          // the offset only after UTC or GMT; the words stop at 'to'</para>
///     <para>{Expression} ::= {Term} (('+' | '-') {Term})*</para>
///     <para>{Term}       ::= {Unary} (('*' | '/' | '%') {Unary})*</para>
///     <para>{Unary}      ::= ('+' | '-') {Unary} | {Power}</para>
///     <para>{Power}      ::= {Primary} ('^' {Unary})?</para>
///     <para>{Primary}    ::= {Number}
///                          | {String}
///                          | {Identifier} ('(' ({Expression} (',' {Expression})*)? ')')?
///                          | '(' {Expression} ')'</para>
///     <para>Power is right-associative and binds tighter than a leading sign: 2^3^2 = 2^9, -2^2 = -4, 2^-1 = 0.5.</para>
/// </summary>
public class ExpressionParser
{
    private const string NowWord = "now";

    private IReadOnlyList<Token> _tokens = [];
    private int _index;

    private Token Current => _tokens[_index];

    private Token Next => _tokens[Math.Min(_index + 1, _tokens.Count - 1)];

    /// <summary>
    ///     Parses the tokens of a whole expression.
    /// </summary>
    /// <param name="tokens">Tokens, ending with <see cref="TokenType.EndOfExpression"/>.</param>
    /// <exception cref="ExpressionException">The expression is empty or has a syntax error.</exception>
    public ExpressionNode Parse(IReadOnlyList<Token> tokens)
    {
        if (tokens.Count == 0 || tokens[^1].Type != TokenType.EndOfExpression)
        {
            throw new ArgumentException("Tokens must end with EndOfExpression.", nameof(tokens));
        }

        _tokens = tokens;
        _index = 0;

        if (Current.Type == TokenType.EndOfExpression)
        {
            throw new ExpressionException("Empty expression", Current.Position);
        }

        ExpressionNode node = StartsWithTime() ? ParseTimeConversion() : ParseExpression();

        if (node is not TimeConversionNode && Current.Type == TokenType.Identifier)
        {
            node = ParseConversion(node);
        }

        if (Current.Type != TokenType.EndOfExpression)
        {
            throw new ExpressionException($"Unexpected '{Current.Value}'", Current.Position);
        }

        return node;
    }

    private ConversionNode ParseConversion(ExpressionNode value)
    {
        Token fromToken = Current;

        if (IsToWord(fromToken))
        {
            throw new ExpressionException("Expected a unit before 'to'", fromToken.Position);
        }

        string from = ParseUnit();

        if (!IsToWord(Current))
        {
            throw new ExpressionException($"Expected 'to' after '{from}'", fromToken.Position);
        }

        Token toWord = Current;
        Advance();

        Token toToken = Current;

        if (toToken.Type != TokenType.Identifier)
        {
            throw new ExpressionException("Expected a unit after 'to'", toToken.Position);
        }

        return new ConversionNode(value, from, fromToken.Position, ParseUnit(), toToken.Position, toWord.Position);
    }

    /// <summary>
    ///     Reads a unit's name, which can have an operator in it: km/h, m^2.
    /// </summary>
    private string ParseUnit()
    {
        string name = Current.Value;
        Advance();

        Token next = _tokens[Math.Min(_index + 1, _tokens.Count - 1)];

        if ((Current.Type == TokenType.Divide && next.Type == TokenType.Identifier)
            || (Current.Type == TokenType.Power && next.Type == TokenType.Number))
        {
            name += Current.Value + next.Value;
            Advance();
            Advance();
        }

        return name;
    }

    /// <summary>
    ///     A time conversion starts with a time (10:30, 3 pm) or 'now'. A number without am or pm is a number:
    ///     3 h to min.
    /// </summary>
    private bool StartsWithTime() => Current.Type switch
    {
        TokenType.Time => true,
        TokenType.Number => IsHalfOfDay(Next),
        TokenType.Identifier => Current.Value.Equals(NowWord, StringComparison.OrdinalIgnoreCase)
            && Next.Type is TokenType.Identifier or TokenType.EndOfExpression,
        _ => false,
    };

    private TimeConversionNode ParseTimeConversion()
    {
        Token timeToken = Current;
        TimeOnly? time = null;
        bool twelveHour = false;

        if (timeToken.Type == TokenType.Identifier)
        {
            // now
            Advance();
        }
        else
        {
            (time, twelveHour) = ParseTime();
        }

        string? from = null;
        Token fromToken = Current;

        if (!IsToWord(fromToken))
        {
            if (fromToken.Type != TokenType.Identifier)
            {
                throw new ExpressionException("Expected a time zone or 'to' after the time", fromToken.Position);
            }

            from = ParseTimeZone();

            // The time zone's words go on until 'to', so only something else can stop them
            if (!IsToWord(Current))
            {
                throw new ExpressionException($"Expected 'to' after '{from}'", Current.Position);
            }
        }

        Token toWord = Current;
        Advance();

        Token toToken = Current;

        if (toToken.Type != TokenType.Identifier)
        {
            throw new ExpressionException("Expected a time zone after 'to'", toToken.Position);
        }

        string to = ParseTimeZone();

        return new TimeConversionNode(
            time,
            twelveHour,
            timeToken.Position,
            from,
            fromToken.Position,
            to,
            toToken.Position,
            toWord.Position);
    }

    /// <summary>
    ///     Reads a time: 10:30 or 22:30 (24-hour), or 10 pm or 10:30 pm (12-hour).
    /// </summary>
    private (TimeOnly Time, bool TwelveHour) ParseTime()
    {
        Token token = Current;
        Advance();

        string[] parts = token.Value.Split(':');

        if (parts[0].Contains('.'))
        {
            throw new ExpressionException("Write a time as 10:30 or 10:30 pm", token.Position);
        }

        bool twelveHour = IsHalfOfDay(Current);
        bool pm = twelveHour && Current.Value.Equals("pm", StringComparison.OrdinalIgnoreCase);

        if (twelveHour)
        {
            Advance();
        }

        int lowest = twelveHour ? 1 : 0;
        int highest = twelveHour ? 12 : 23;

        if (!int.TryParse(parts[0], CultureInfo.InvariantCulture, out int hours) || hours < lowest || hours > highest)
        {
            string message = twelveHour
                ? "With am or pm, the hour must be from 1 to 12"
                : "The hour must be from 0 to 23";
            throw new ExpressionException(message, token.Position);
        }

        int minutes = 0;

        if (parts.Length > 1)
        {
            int minutesPosition = token.Position + parts[0].Length + 1;

            if (parts[1].Length != 2 || !int.TryParse(parts[1], CultureInfo.InvariantCulture, out minutes)
                || minutes > 59)
            {
                throw new ExpressionException("The minutes must be two digits, from 00 to 59", minutesPosition);
            }
        }

        if (twelveHour)
        {
            // 12 am is midnight and 12 pm is noon
            hours = hours % 12 + (pm ? 12 : 0);
        }

        return (new TimeOnly(hours, minutes), twelveHour);
    }

    /// <summary>
    ///     Reads a place or time zone, which can have several words (Sao Paulo), slashes (America/Lima) and, after
    ///     UTC or GMT, an offset (UTC-5, UTC+5:30).
    /// </summary>
    private string ParseTimeZone()
    {
        var name = new StringBuilder(Current.Value);
        Advance();

        while (true)
        {
            if (Current.Type == TokenType.Divide && Next.Type == TokenType.Identifier)
            {
                name.Append('/').Append(Next.Value);
                Advance();
                Advance();
            }
            else if (Current.Type == TokenType.Identifier && !IsToWord(Current))
            {
                name.Append(' ').Append(Current.Value);
                Advance();
            }
            else
            {
                break;
            }
        }

        string text = name.ToString();
        bool isUtc = text.Equals("utc", StringComparison.OrdinalIgnoreCase)
            || text.Equals("gmt", StringComparison.OrdinalIgnoreCase);

        if (isUtc && Current.Type is TokenType.Plus or TokenType.Minus
            && Next.Type is TokenType.Number or TokenType.Time)
        {
            name.Append(Current.Value).Append(Next.Value);
            Advance();
            Advance();
        }

        return name.ToString();
    }

    private static bool IsHalfOfDay(Token token) =>
        token.Type == TokenType.Identifier
        && (token.Value.Equals("am", StringComparison.OrdinalIgnoreCase)
            || token.Value.Equals("pm", StringComparison.OrdinalIgnoreCase));

    private static bool IsToWord(Token token) =>
        token.Type == TokenType.Identifier && token.Value.Equals("to", StringComparison.OrdinalIgnoreCase);

    private ExpressionNode ParseExpression()
    {
        ExpressionNode left = ParseTerm();

        while (Current.Type is TokenType.Plus or TokenType.Minus)
        {
            Token op = Current;
            Advance();

            ExpressionNode right = ParseTerm();

            BinaryOperator binaryOperator = op.Type == TokenType.Plus ? BinaryOperator.Add : BinaryOperator.Subtract;
            left = new BinaryNode(binaryOperator, left, right, op.Position);
        }

        return left;
    }

    private ExpressionNode ParseTerm()
    {
        ExpressionNode left = ParseUnary();

        while (Current.Type is TokenType.Multiply or TokenType.Divide or TokenType.Modulo)
        {
            Token op = Current;
            Advance();

            ExpressionNode right = ParseUnary();

            BinaryOperator binaryOperator = op.Type switch
            {
                TokenType.Multiply => BinaryOperator.Multiply,
                TokenType.Divide => BinaryOperator.Divide,
                _ => BinaryOperator.Modulo
            };
            left = new BinaryNode(binaryOperator, left, right, op.Position);
        }

        return left;
    }

    private ExpressionNode ParseUnary()
    {
        if (Current.Type is TokenType.Plus or TokenType.Minus)
        {
            Token op = Current;
            Advance();

            ExpressionNode operand = ParseUnary();

            UnaryOperator unaryOperator = op.Type == TokenType.Plus ? UnaryOperator.Plus : UnaryOperator.Negate;
            return new UnaryNode(unaryOperator, operand, op.Position);
        }

        return ParsePower();
    }

    private ExpressionNode ParsePower()
    {
        ExpressionNode left = ParsePrimary();

        if (Current.Type == TokenType.Power)
        {
            Token op = Current;
            Advance();

            // Unary (not Primary) makes ^ right-associative and allows a signed exponent: 2^3^2, 2^-1
            ExpressionNode right = ParseUnary();

            return new BinaryNode(BinaryOperator.Power, left, right, op.Position);
        }

        return left;
    }

    private ExpressionNode ParsePrimary()
    {
        Token token = Current;

        switch (token.Type)
        {
            case TokenType.Number:
                Advance();
                // Already validated by the tokenizer
                return new NumberNode(
                    decimal.Parse(token.Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture),
                    token.Position);

            case TokenType.String:
                // Allowed anywhere by the grammar; the evaluator only accepts it as a function argument
                Advance();
                return new StringNode(token.Value, token.Position);

            case TokenType.Identifier:
                Advance();
                return Current.Type == TokenType.LeftParenthesis
                    ? ParseFunctionCall(token)
                    : new IdentifierNode(token.Value, token.Position);

            case TokenType.LeftParenthesis:
                Advance();
                ExpressionNode node = ParseExpression();
                Expect(TokenType.RightParenthesis, "Expected ')'");
                return node;

            case TokenType.EndOfExpression:
                throw new ExpressionException("Unexpected end of expression", token.Position);

            default:
                throw new ExpressionException($"Unexpected '{token.Value}'", token.Position);
        }
    }

    private FunctionCallNode ParseFunctionCall(Token name)
    {
        // Skip '('
        Advance();

        var arguments = new List<ExpressionNode>();

        if (Current.Type != TokenType.RightParenthesis)
        {
            arguments.Add(ParseExpression());

            while (Current.Type == TokenType.Comma)
            {
                Advance();
                arguments.Add(ParseExpression());
            }
        }

        Expect(TokenType.RightParenthesis, arguments.Count == 0 ? "Expected ')'" : "Expected ',' or ')'");

        return new FunctionCallNode(name.Value, arguments, name.Position);
    }

    private void Expect(TokenType type, string message)
    {
        if (Current.Type != type)
        {
            throw new ExpressionException(message, Current.Position);
        }

        Advance();
    }

    private void Advance()
    {
        // Never move past EndOfExpression
        if (_index < _tokens.Count - 1)
        {
            _index++;
        }
    }
}
