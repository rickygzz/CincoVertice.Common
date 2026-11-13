namespace CincoVertice.Common.Application.Scripting.Functions;

/// <param name="Name">Name used in scripts (case-insensitive).</param>
/// <param name="MinArguments">Minimum number of arguments.</param>
/// <param name="MaxArguments">Maximum number of arguments.</param>
/// <param name="Invoke">
///     Implementation. Returns a decimal, string or bool, or <see cref="ScriptValue.Nothing"/>. Other exceptions than
///     <see cref="ScriptException"/> and <see cref="ScriptStopException"/> are reported as errors at the call.
/// </param>
public record ScriptFunction(
    string Name,
    int MinArguments,
    int MaxArguments,
    Func<ScriptArguments, Task<object>> Invoke)
{
    public bool AcceptsArgumentCount(int count) => count >= MinArguments && count <= MaxArguments;

    /// <summary>E.g. "1 argument", "1 to 2 arguments", "no arguments".</summary>
    public string DescribeArgumentCount()
    {
        if (MinArguments == MaxArguments)
        {
            return Plural(MinArguments);
        }

        return $"{MinArguments} to {Plural(MaxArguments)}";
    }

    private static string Plural(int count) => count switch
    {
        0 => "no arguments",
        1 => "1 argument",
        _ => $"{count} arguments"
    };
}
