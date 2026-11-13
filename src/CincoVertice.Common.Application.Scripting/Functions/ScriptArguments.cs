namespace CincoVertice.Common.Application.Scripting.Functions;

/// <summary>
///     The evaluated arguments of a function call, with typed accessors that report a wrong type at the call.
/// </summary>
public class ScriptArguments
{
    private readonly IReadOnlyList<object> _values;
    private readonly string _function;
    private readonly int _line;
    private readonly int _column;

    public ScriptArguments(IReadOnlyList<object> values, string function, int line, int column)
    {
        _values = values;
        _function = function;
        _line = line;
        _column = column;
    }

    public int Count => _values.Count;

    /// <summary>Argument i as text. Numbers and true/false are converted, so say(5) works.</summary>
    public string Text(int i) => ScriptValue.ToText(_values[i]);

    /// <summary>Argument i as text, or <paramref name="defaultText"/> when it was not given.</summary>
    public string Text(int i, string defaultText) => i < Count ? Text(i) : defaultText;

    /// <exception cref="ScriptException">The argument is not a number.</exception>
    public decimal Number(int i)
    {
        if (_values[i] is decimal number)
        {
            return number;
        }

        throw Error($"argument {i + 1} must be a number, not {ScriptValue.Describe(_values[i])}");
    }

    /// <exception cref="ScriptException">The argument is not a list.</exception>
    public IReadOnlyList<object> List(int i)
    {
        if (_values[i] is IReadOnlyList<object> list)
        {
            return list;
        }

        throw Error($"argument {i + 1} must be a list, not {ScriptValue.Describe(_values[i])}");
    }

    /// <summary>Argument i, whatever its type.</summary>
    public object Value(int i) => _values[i];

    /// <summary>An error at this call, e.g. "wait(): argument 1 must be a number".</summary>
    public ScriptException Error(string message) => new($"{_function}(): {message}", _line, _column);
}
