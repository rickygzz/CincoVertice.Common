using System.Globalization;
using CincoVertice.Common.Application.Scripting.Ast;
using CincoVertice.Common.Application.Scripting.Models;

namespace CincoVertice.Common.Application.Scripting;

/// <summary>
///     Builds the statements of a script from the tokens of <see cref="ScriptLexer"/> (recursive descent).
///     <para>{Script}     ::= {Statement}*</para>
///     <para>{Statement}  ::= ({Identifier} '=' {Expression} | {Call} | {If}) NewLine</para>
///     <para>{If}         ::= 'if' {Expression} NewLine {Statement}* ('elseif' {Expression} NewLine {Statement}*)*
///                            ('else' NewLine {Statement}*)? 'end'</para>
///     <para>{Expression} ::= {And} ('or' {And})*</para>
///     <para>{And}        ::= {Not} ('and' {Not})*</para>
///     <para>{Not}        ::= 'not' {Not} | {Comparison}</para>
///     <para>{Comparison} ::= {Sum} (('==' | '!=' | '&lt;' | '&lt;=' | '&gt;' | '&gt;=') {Sum})?</para>
///     <para>{Sum}        ::= {Product} (('+' | '-') {Product})*</para>
///     <para>{Product}    ::= {Unary} (('*' | '/' | '%') {Unary})*</para>
///     <para>{Unary}      ::= '-' {Unary} | {Postfix}</para>
///     <para>{Postfix}    ::= {Primary} ('[' {Expression} ']')*</para>
///     <para>{Primary}    ::= {Number} | {Text} | 'true' | 'false' | {Identifier} | {Call} | {List}
///                          | '(' {Expression} ')'</para>
///     <para>{Call}       ::= {Identifier} '(' ({Expression} (',' {Expression})*)? ')'</para>
///     <para>{List}       ::= '[' ({Expression} (',' {Expression})*)? ']'</para>
/// </summary>
public class ScriptParser
{
    private static readonly Dictionary<string, BinaryOperator> _comparisons = new()
    {
        ["=="] = BinaryOperator.Equal,
        ["!="] = BinaryOperator.NotEqual,
        ["<"] = BinaryOperator.Less,
        ["<="] = BinaryOperator.LessOrEqual,
        [">"] = BinaryOperator.Greater,
        [">="] = BinaryOperator.GreaterOrEqual,
    };

    private IReadOnlyList<Token> _tokens = [];
    private int _index;

    private Token Current => _tokens[_index];

    /// <summary>
    ///     Parses a whole script, so syntax errors are found before anything runs.
    /// </summary>
    /// <param name="tokens">Tokens, ending with <see cref="TokenType.EndOfScript"/>.</param>
    /// <exception cref="ScriptException">The script has a syntax error.</exception>
    public IReadOnlyList<Statement> Parse(IReadOnlyList<Token> tokens)
    {
        if (tokens.Count == 0 || tokens[^1].Type != TokenType.EndOfScript)
        {
            throw new ArgumentException("Tokens must end with EndOfScript.", nameof(tokens));
        }

        _tokens = tokens;
        _index = 0;

        return ParseBlock();
    }

    /// <summary>
    ///     Statements up to the end of the script or one of the given keywords, which is left for the caller.
    /// </summary>
    private List<Statement> ParseBlock(params string[] terminators)
    {
        List<Statement> statements = [];

        while (Current.Type != TokenType.EndOfScript && !IsKeyword(terminators))
        {
            if (IsKeyword("elseif", "else", "end"))
            {
                throw Error($"'{Current.Value}' without an 'if'");
            }

            statements.Add(ParseStatement());
        }

        return statements;
    }

    private Statement ParseStatement()
    {
        Token start = Current;
        Statement statement;

        if (IsKeyword("if"))
        {
            statement = ParseIf();
        }
        else if (start.Type == TokenType.Identifier && Peek(1) is { Type: TokenType.Operator, Value: "=" })
        {
            if (start.Value.Contains('.'))
            {
                throw Error($"A variable name can't contain '.': '{start.Value}' looks like a function");
            }

            _index += 2;
            statement = new AssignmentStatement(start.Value, ParseExpression(), start.Line, start.Column);
        }
        else
        {
            var expression = ParseExpression();

            if (expression is not CallExpression call)
            {
                throw new ScriptException(
                    "A line must assign a value (name = ...) or call a function (name(...))",
                    start.Line,
                    start.Column);
            }

            statement = new CallStatement(call, start.Line, start.Column);
        }

        ExpectEndOfStatement();

        return statement;
    }

    private IfStatement ParseIf()
    {
        Token start = Current;
        List<IfBranch> branches = [];
        List<Statement> elseBody = [];

        do
        {
            // Skip "if" or "elseif"
            _index++;
            var condition = ParseExpression();
            ExpectEndOfStatement();
            branches.Add(new IfBranch(condition, ParseBlock("elseif", "else", "end")));
        }
        while (IsKeyword("elseif"));

        if (IsKeyword("else"))
        {
            _index++;
            ExpectEndOfStatement();
            elseBody = ParseBlock("end");
        }

        if (!IsKeyword("end"))
        {
            throw Error($"Missing 'end' for the 'if' on line {start.Line}");
        }

        _index++;

        return new IfStatement(branches, elseBody, start.Line, start.Column);
    }

    private Expression ParseExpression()
    {
        var left = ParseAnd();

        while (IsKeyword("or"))
        {
            Token op = Next();
            left = new BinaryExpression(BinaryOperator.Or, left, ParseAnd(), op.Line, op.Column);
        }

        return left;
    }

    private Expression ParseAnd()
    {
        var left = ParseNot();

        while (IsKeyword("and"))
        {
            Token op = Next();
            left = new BinaryExpression(BinaryOperator.And, left, ParseNot(), op.Line, op.Column);
        }

        return left;
    }

    private Expression ParseNot()
    {
        if (IsKeyword("not"))
        {
            Token op = Next();

            return new UnaryExpression(UnaryOperator.Not, ParseNot(), op.Line, op.Column);
        }

        return ParseComparison();
    }

    private Expression ParseComparison()
    {
        var left = ParseSum();

        if (Current.Type == TokenType.Operator && _comparisons.TryGetValue(Current.Value, out var comparison))
        {
            Token op = Next();
            left = new BinaryExpression(comparison, left, ParseSum(), op.Line, op.Column);
        }

        if (Current.Type == TokenType.Operator && _comparisons.ContainsKey(Current.Value))
        {
            throw Error("Compare two values at a time: use 'and', as in a < b and b < c");
        }

        if (Current is { Type: TokenType.Operator, Value: "=" })
        {
            throw Error(left is IndexExpression
                ? "A list's items can't be changed: build a new list, e.g. names = [first, second]"
                : "Use == to compare; = only assigns a value at the start of a line");
        }

        return left;
    }

    private Expression ParseSum()
    {
        var left = ParseProduct();

        while (Current is { Type: TokenType.Operator, Value: "+" or "-" })
        {
            Token op = Next();
            var kind = op.Value == "+" ? BinaryOperator.Add : BinaryOperator.Subtract;
            left = new BinaryExpression(kind, left, ParseProduct(), op.Line, op.Column);
        }

        return left;
    }

    private Expression ParseProduct()
    {
        var left = ParseUnary();

        while (Current is { Type: TokenType.Operator, Value: "*" or "/" or "%" })
        {
            Token op = Next();
            var kind = op.Value switch
            {
                "*" => BinaryOperator.Multiply,
                "/" => BinaryOperator.Divide,
                _ => BinaryOperator.Modulo
            };
            left = new BinaryExpression(kind, left, ParseUnary(), op.Line, op.Column);
        }

        return left;
    }

    private Expression ParseUnary()
    {
        if (Current is { Type: TokenType.Operator, Value: "-" })
        {
            Token op = Next();

            return new UnaryExpression(UnaryOperator.Negate, ParseUnary(), op.Line, op.Column);
        }

        return ParsePostfix();
    }

    private Expression ParsePostfix()
    {
        var expression = ParsePrimary();

        while (Current.Type == TokenType.LeftBracket)
        {
            Token bracket = Next();
            var index = ParseExpression();
            Expect(TokenType.RightBracket, "Missing ']'");
            expression = new IndexExpression(expression, index, bracket.Line, bracket.Column);
        }

        return expression;
    }

    private Expression ParsePrimary()
    {
        Token token = Current;

        switch (token.Type)
        {
            case TokenType.Number:
                _index++;

                return new NumberExpression(
                    decimal.Parse(token.Value, CultureInfo.InvariantCulture), token.Line, token.Column);

            case TokenType.Text:
                _index++;

                return new TextExpression(token.Value, token.Line, token.Column);

            case TokenType.Keyword when token.Value is "true" or "false":
                _index++;

                return new BooleanExpression(token.Value == "true", token.Line, token.Column);

            case TokenType.Identifier when Peek(1).Type == TokenType.LeftParenthesis:
                return ParseCall();

            case TokenType.Identifier:
                _index++;

                return new VariableExpression(token.Value, token.Line, token.Column);

            case TokenType.LeftParenthesis:
                _index++;
                var inner = ParseExpression();
                Expect(TokenType.RightParenthesis, "Missing ')'");

                return inner;

            case TokenType.LeftBracket:
                _index++;
                var items = ParseItems(TokenType.RightBracket);
                Expect(TokenType.RightBracket, "Missing ']' to close the list");

                return new ListExpression(items, token.Line, token.Column);

            default:
                throw Error(token.Type is TokenType.NewLine or TokenType.EndOfScript
                    ? "Expected a value before the end of the line"
                    : $"Expected a value, not '{token.Value}'");
        }
    }

    private CallExpression ParseCall()
    {
        Token name = Current;

        // Skip the name and "("
        _index += 2;
        var arguments = ParseItems(TokenType.RightParenthesis);
        Expect(TokenType.RightParenthesis, $"Missing ')' to close {name.Value}(");

        return new CallExpression(name.Value, arguments, name.Line, name.Column);
    }

    /// <summary>
    ///     Comma-separated expressions up to <paramref name="end"/>, which is left for the caller.
    /// </summary>
    private List<Expression> ParseItems(TokenType end)
    {
        List<Expression> items = [];

        if (Current.Type == end)
        {
            return items;
        }

        items.Add(ParseExpression());

        while (Current.Type == TokenType.Comma)
        {
            _index++;
            items.Add(ParseExpression());
        }

        return items;
    }

    private void ExpectEndOfStatement()
    {
        if (Current.Type == TokenType.NewLine)
        {
            _index++;
        }
        else if (Current.Type != TokenType.EndOfScript)
        {
            throw Error($"Expected the end of the line, not '{Current.Value}'");
        }
    }

    private void Expect(TokenType type, string message)
    {
        if (Current.Type != type)
        {
            throw Error(message);
        }

        _index++;
    }

    private Token Next() => _tokens[_index++];

    private Token Peek(int offset) => _tokens[Math.Min(_index + offset, _tokens.Count - 1)];

    private bool IsKeyword(params string[] keywords) =>
        Current.Type == TokenType.Keyword && keywords.Contains(Current.Value);

    private ScriptException Error(string message) => new(message, Current.Line, Current.Column);
}
