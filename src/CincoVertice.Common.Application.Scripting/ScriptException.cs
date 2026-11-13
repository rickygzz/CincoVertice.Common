namespace CincoVertice.Common.Application.Scripting;

/// <summary>
///     A syntax or run-time error in a script, with the position it was found at.
/// </summary>
public class ScriptException : Exception
{
    public ScriptException(string message, int line, int column)
        : base(message)
    {
        Line = line;
        Column = column;
    }

    /// <summary>1-based line of the error.</summary>
    public int Line { get; }

    /// <summary>1-based column of the error.</summary>
    public int Column { get; }

    /// <summary>The message with its position, e.g. "Line 3, column 5: Unknown variable 'nmae'".</summary>
    public string Description => $"Line {Line}, column {Column}: {Message}";
}
