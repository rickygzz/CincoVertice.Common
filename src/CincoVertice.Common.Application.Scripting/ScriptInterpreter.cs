using CincoVertice.Common.Application.Scripting.Ast;
using CincoVertice.Common.Application.Scripting.Functions;

namespace CincoVertice.Common.Application.Scripting;

/// <summary>
///     Runs the statements built by <see cref="ScriptParser"/>. Variable and function names are case-insensitive.
///     <para>+ adds numbers, and joins texts when either side is a text: "Hi " + 2 is "Hi 2". == and != compare any
///     values (a text never equals a number); &lt; &lt;= &gt; &gt;= compare two numbers or two texts. Conditions,
///     and, or and not need true or false.</para>
/// </summary>
public class ScriptInterpreter
{
    private readonly IReadOnlyDictionary<string, ScriptFunction> _functions;
    private readonly IReadOnlyDictionary<string, string> _renamed;
    private readonly Dictionary<string, object> _variables = new(StringComparer.OrdinalIgnoreCase);

    /// <param name="functions">The functions scripts can call, by name.</param>
    /// <param name="renamed">Old function names and what replaced them, for error hints; none by default.</param>
    public ScriptInterpreter(
        IReadOnlyDictionary<string, ScriptFunction> functions,
        IReadOnlyDictionary<string, string>? renamed = null)
    {
        _functions = functions;
        _renamed = renamed ?? new Dictionary<string, string>();
    }

    /// <summary>
    ///     Runs the statements in order. stop(), or cancelling ask(), ends the script early without an error.
    /// </summary>
    /// <exception cref="ScriptException">A run-time error, such as an unknown variable or a wrong type.</exception>
    public async Task RunAsync(IReadOnlyList<Statement> statements)
    {
        try
        {
            await ExecuteBlockAsync(statements);
        }
        catch (ScriptStopException)
        {
            // Ended on purpose
        }
    }

    /// <summary>
    ///     Runs the statements of an included script with this script's variables, so each sees what the other sets.
    ///     A stop() in them ends the whole script.
    /// </summary>
    public Task RunIncludedAsync(IReadOnlyList<Statement> statements) => ExecuteBlockAsync(statements);

    private async Task ExecuteBlockAsync(IReadOnlyList<Statement> statements)
    {
        foreach (var statement in statements)
        {
            await ExecuteAsync(statement);
        }
    }

    private async Task ExecuteAsync(Statement statement)
    {
        switch (statement)
        {
            case AssignmentStatement assignment:
                object value = await EvaluateAsync(assignment.Value);

                if (value == ScriptValue.Nothing)
                {
                    throw new ScriptException(
                        $"{((CallExpression)assignment.Value).Name}() returns no value to store",
                        assignment.Value.Line,
                        assignment.Value.Column);
                }

                _variables[assignment.Name] = value;
                break;

            case CallStatement call:
                await EvaluateAsync(call.Call);
                break;

            case IfStatement ifStatement:
                await ExecuteIfAsync(ifStatement);
                break;

            default:
                throw new ArgumentException($"Unknown statement '{statement.GetType().Name}'", nameof(statement));
        }
    }

    private async Task ExecuteIfAsync(IfStatement ifStatement)
    {
        foreach (var branch in ifStatement.Branches)
        {
            if (await EvaluateConditionAsync(branch.Condition, "The condition"))
            {
                await ExecuteBlockAsync(branch.Body);

                return;
            }
        }

        await ExecuteBlockAsync(ifStatement.Else);
    }

    private async Task<object> EvaluateAsync(Expression expression)
    {
        object value = expression switch
        {
            NumberExpression number => number.Value,
            TextExpression text => text.Value,
            BooleanExpression boolean => boolean.Value,
            VariableExpression variable => GetVariable(variable),
            UnaryExpression unary => await EvaluateUnaryAsync(unary),
            BinaryExpression binary => await EvaluateBinaryAsync(binary),
            CallExpression call => await CallAsync(call),
            ListExpression list => await EvaluateListAsync(list),
            IndexExpression index => await EvaluateIndexAsync(index),
            _ => throw new ArgumentException($"Unknown expression '{expression.GetType().Name}'", nameof(expression))
        };

        return value;
    }

    /// <summary>
    ///     Evaluates an operand, which must have a value: say("x") + 1 is an error.
    /// </summary>
    private async Task<object> EvaluateOperandAsync(Expression expression)
    {
        object value = await EvaluateAsync(expression);

        if (value == ScriptValue.Nothing)
        {
            throw new ScriptException(
                $"{((CallExpression)expression).Name}() returns no value to use",
                expression.Line,
                expression.Column);
        }

        return value;
    }

    private async Task<bool> EvaluateConditionAsync(Expression expression, string what)
    {
        object value = await EvaluateOperandAsync(expression);

        return value as bool? ?? throw new ScriptException(
            $"{what} must be true or false, not {ScriptValue.Describe(value)}",
            expression.Line,
            expression.Column);
    }

    private async Task<object> EvaluateListAsync(ListExpression list)
    {
        List<object> items = [];

        foreach (var item in list.Items)
        {
            items.Add(await EvaluateOperandAsync(item));
        }

        return items.AsReadOnly();
    }

    /// <summary>
    ///     list[index], where the first item is 1.
    /// </summary>
    private async Task<object> EvaluateIndexAsync(IndexExpression index)
    {
        object target = await EvaluateOperandAsync(index.List);
        object position = await EvaluateOperandAsync(index.Index);

        if (target is not IReadOnlyList<object> list)
        {
            throw new ScriptException(
                $"Only a list has items to pick with [ ], not {ScriptValue.Describe(target)}",
                index.Line,
                index.Column);
        }

        if (position is not decimal number || number != decimal.Truncate(number))
        {
            throw new ScriptException(
                $"The position in [ ] must be a whole number, not {ScriptValue.ToText(position)}",
                index.Index.Line,
                index.Index.Column);
        }

        if (number < 1 || number > list.Count)
        {
            string items = list.Count == 1 ? "1 item" : $"{list.Count} items";

            throw new ScriptException(
                $"There is no item {ScriptValue.ToText(number)}: the list has {items}, and the first is 1",
                index.Index.Line,
                index.Index.Column);
        }

        return list[(int)number - 1];
    }

    private object GetVariable(VariableExpression variable)
    {
        if (_variables.TryGetValue(variable.Name, out object? value))
        {
            return value;
        }

        string hint = string.Empty;

        if (_functions.ContainsKey(variable.Name))
        {
            hint = $": to call the function, write {variable.Name}()";
        }
        else if (_renamed.TryGetValue(variable.Name, out string? newName))
        {
            hint = $": the function {variable.Name}() is now {newName}";
        }

        throw new ScriptException($"Unknown variable '{variable.Name}'{hint}", variable.Line, variable.Column);
    }

    /// <summary>
    ///     Why a function name is unknown: an old name and its replacement, or the functions of its module.
    /// </summary>
    private string UnknownFunctionHint(string name)
    {
        if (_renamed.TryGetValue(name, out string? newName))
        {
            return $": it is now {newName}";
        }

        int dot = name.LastIndexOf('.');

        if (dot < 0)
        {
            return string.Empty;
        }

        string module = name[..(dot + 1)];
        List<string> moduleFunctions =
        [
            .. _functions.Keys
                .Where(key => key.StartsWith(module, StringComparison.OrdinalIgnoreCase))
                .Select(key => key[module.Length..])
                .Order(StringComparer.Ordinal)
        ];

        return moduleFunctions.Count > 0
            ? $": {name[..dot]} has {string.Join(", ", moduleFunctions)}"
            : $": there is no module '{name[..dot]}'";
    }

    private async Task<object> EvaluateUnaryAsync(UnaryExpression unary)
    {
        if (unary.Operator == UnaryOperator.Not)
        {
            return !await EvaluateConditionAsync(unary.Operand, "The value after 'not'");
        }

        object operand = await EvaluateOperandAsync(unary.Operand);

        return operand as decimal? is decimal number
            ? -number
            : throw new ScriptException(
                $"'-' needs a number, not {ScriptValue.Describe(operand)}", unary.Line, unary.Column);
    }

    private async Task<object> EvaluateBinaryAsync(BinaryExpression binary)
    {
        // and / or stop as soon as the result is known
        if (binary.Operator is BinaryOperator.And or BinaryOperator.Or)
        {
            string name = binary.Operator == BinaryOperator.And ? "'and'" : "'or'";
            bool left = await EvaluateConditionAsync(binary.Left, $"The value before {name}");

            if (left == (binary.Operator == BinaryOperator.Or))
            {
                return left;
            }

            return await EvaluateConditionAsync(binary.Right, $"The value after {name}");
        }

        object leftValue = await EvaluateOperandAsync(binary.Left);
        object rightValue = await EvaluateOperandAsync(binary.Right);

        try
        {
            return Calculate(binary, leftValue, rightValue);
        }
        catch (OverflowException)
        {
            throw new ScriptException("The number is too large", binary.Line, binary.Column);
        }
    }

    private static object Calculate(BinaryExpression binary, object left, object right)
    {
        bool isAdd = binary.Operator == BinaryOperator.Add;

        if (isAdd && left is IReadOnlyList<object> leftList && right is IReadOnlyList<object> rightList)
        {
            return (IReadOnlyList<object>)[.. leftList, .. rightList];
        }

        switch (binary.Operator)
        {
            case BinaryOperator.Add when left is string || right is string:
                return ScriptValue.ToText(left) + ScriptValue.ToText(right);

            case BinaryOperator.Equal:
                return ScriptValue.AreEqual(left, right);

            case BinaryOperator.NotEqual:
                return !ScriptValue.AreEqual(left, right);
        }

        if (binary.Operator is BinaryOperator.Less or BinaryOperator.LessOrEqual or BinaryOperator.Greater
            or BinaryOperator.GreaterOrEqual)
        {
            return CompareOrder(binary, Compare(binary, left, right));
        }

        if (left is not decimal a || right is not decimal b)
        {
            string wrongType = ScriptValue.Describe(left is decimal ? right : left);

            throw new ScriptException(
                $"'{Symbol(binary.Operator)}' needs numbers, not {wrongType}",
                binary.Line,
                binary.Column);
        }

        if (b == 0 && binary.Operator is BinaryOperator.Divide or BinaryOperator.Modulo)
        {
            throw new ScriptException("Division by zero", binary.Line, binary.Column);
        }

        return binary.Operator switch
        {
            BinaryOperator.Add => a + b,
            BinaryOperator.Subtract => a - b,
            BinaryOperator.Multiply => a * b,
            BinaryOperator.Divide => a / b,
            BinaryOperator.Modulo => a % b,
            _ => throw new ArgumentOutOfRangeException(nameof(binary), binary.Operator, null)
        };
    }

    private static int Compare(BinaryExpression binary, object left, object right)
    {
        if (left is decimal leftNumber && right is decimal rightNumber)
        {
            return leftNumber.CompareTo(rightNumber);
        }

        if (left is string leftText && right is string rightText)
        {
            return string.CompareOrdinal(leftText, rightText);
        }

        throw new ScriptException(
            $"'{Symbol(binary.Operator)}' compares two numbers or two texts, not {ScriptValue.Describe(left)} "
            + $"and {ScriptValue.Describe(right)}",
            binary.Line,
            binary.Column);
    }

    private static bool CompareOrder(BinaryExpression binary, int comparison) => binary.Operator switch
    {
        BinaryOperator.Less => comparison < 0,
        BinaryOperator.LessOrEqual => comparison <= 0,
        BinaryOperator.Greater => comparison > 0,
        _ => comparison >= 0
    };

    private static string Symbol(BinaryOperator op) => op switch
    {
        BinaryOperator.Add => "+",
        BinaryOperator.Subtract => "-",
        BinaryOperator.Multiply => "*",
        BinaryOperator.Divide => "/",
        BinaryOperator.Modulo => "%",
        BinaryOperator.Less => "<",
        BinaryOperator.LessOrEqual => "<=",
        BinaryOperator.Greater => ">",
        _ => ">="
    };

    private async Task<object> CallAsync(CallExpression call)
    {
        if (!_functions.TryGetValue(call.Name, out var function))
        {
            throw new ScriptException(
                $"Unknown function '{call.Name}'{UnknownFunctionHint(call.Name)}", call.Line, call.Column);
        }

        if (!function.AcceptsArgumentCount(call.Arguments.Count))
        {
            throw new ScriptException(
                $"{function.Name}() takes {function.DescribeArgumentCount()}, not {call.Arguments.Count}",
                call.Line,
                call.Column);
        }

        List<object> values = [];

        foreach (var argument in call.Arguments)
        {
            values.Add(await EvaluateOperandAsync(argument));
        }

        try
        {
            return await function.Invoke(new ScriptArguments(values, function.Name, call.Line, call.Column));
        }
        catch (Exception ex) when (ex is not (ScriptException or ScriptStopException))
        {
            // E.g. open() of a file that does not exist
            throw new ScriptException($"{function.Name}(): {ex.Message}", call.Line, call.Column);
        }
    }
}
