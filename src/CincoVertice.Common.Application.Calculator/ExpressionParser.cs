using CincoVertice.Common.Application.Calculator.Ast;
using CincoVertice.Common.Application.Calculator.Models;
using System.Globalization;

namespace CincoVertice.Common.Application.Calculator;

/// <summary>
///     Builds a syntax tree from the tokens returned by <see cref="ExpressionTokenizer"/> (recursive descent).
///     <para>{Calculation} ::= {Expression} ({Unit} 'to' {Unit})?</para>
///     <para>{Unit}       ::= {Identifier} ('/' {Identifier} | '^' {Number})?</para>
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
    private IReadOnlyList<Token> _tokens = [];
    private int _index;

    private Token Current => _tokens[_index];

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

        ExpressionNode node = ParseExpression();

        if (Current.Type == TokenType.Identifier)
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
