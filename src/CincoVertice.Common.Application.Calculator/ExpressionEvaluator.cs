using CincoVertice.Common.Application.Calculator.Ast;
using CincoVertice.Common.Application.Calculator.Functions;

namespace CincoVertice.Common.Application.Calculator;

/// <summary>
///     Calculates the value of a syntax tree built by <see cref="ExpressionParser"/>.
/// </summary>
public class ExpressionEvaluator
{
    /// <exception cref="ExpressionException">
    ///     Division by zero, overflow, unknown name or function, wrong argument count, or invalid function input.
    /// </exception>
    public decimal Evaluate(ExpressionNode node) => node switch
    {
        NumberNode number => number.Value,
        IdentifierNode identifier => EvaluateIdentifier(identifier),
        UnaryNode unary => EvaluateUnary(unary),
        BinaryNode binary => EvaluateBinary(binary),
        FunctionCallNode functionCall => EvaluateFunctionCall(functionCall),
        // A text is only valid as a function argument (see EvaluateArgument)
        StringNode text => throw new ExpressionException("Text can only be used as a function argument", text.Position),
        _ => throw new ArgumentException($"Unknown node type '{node.GetType().Name}'", nameof(node))
    };

    private static decimal EvaluateIdentifier(IdentifierNode node)
    {
        if (BuiltIns.Constants.TryGetValue(node.Name, out decimal value))
        {
            return value;
        }

        throw new ExpressionException($"Unknown name '{node.Name}'", node.Position);
    }

    private decimal EvaluateUnary(UnaryNode node)
    {
        decimal operand = Evaluate(node.Operand);

        // decimal is symmetric, so negation cannot overflow
        return node.Operator == UnaryOperator.Negate ? -operand : operand;
    }

    private decimal EvaluateBinary(BinaryNode node)
    {
        decimal left = Evaluate(node.Left);
        decimal right = Evaluate(node.Right);

        return Calculate(node.Position, () => node.Operator switch
        {
            BinaryOperator.Add => left + right,
            BinaryOperator.Subtract => left - right,
            BinaryOperator.Multiply => left * right,
            BinaryOperator.Divide => left / right,
            // Result has the sign of the dividend: -7 % 3 = -1
            BinaryOperator.Modulo => left % right,
            BinaryOperator.Power => DecimalMath.Pow(left, right),
            _ => throw new ArgumentOutOfRangeException(nameof(node), node.Operator, null)
        });
    }

    private decimal EvaluateFunctionCall(FunctionCallNode node)
    {
        if (!BuiltIns.Functions.TryGetValue(node.Name, out FunctionDefinition? function))
        {
            throw new ExpressionException($"Unknown function '{node.Name}'", node.Position);
        }

        if (!function.AcceptsArgumentCount(node.Arguments.Count))
        {
            throw new ExpressionException(
                $"Function '{function.Name}' expects {function.DescribeArgumentCount()}",
                node.Position);
        }

        List<FunctionArgument> arguments = node.Arguments.Select(EvaluateArgument).ToList();

        return Calculate(node.Position, () => function.Invoke(arguments));
    }

    private FunctionArgument EvaluateArgument(ExpressionNode node) => node is StringNode text
        ? new FunctionArgument(text.Value, text.Position)
        : new FunctionArgument(Evaluate(node), node.Position);

    /// <summary>
    ///     Runs an operation and reports arithmetic errors at the given position.
    /// </summary>
    private static decimal Calculate(int position, Func<decimal> operation)
    {
        try
        {
            return operation();
        }
        catch (DivideByZeroException)
        {
            throw new ExpressionException("Division by zero", position);
        }
        catch (OverflowException)
        {
            throw new ExpressionException("Number is too large", position);
        }
        catch (ArithmeticException ex)
        {
            throw new ExpressionException(ex.Message, position);
        }
    }
}
