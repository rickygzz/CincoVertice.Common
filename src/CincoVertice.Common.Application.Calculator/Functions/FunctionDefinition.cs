namespace CincoVertice.Common.Application.Calculator.Functions;

/// <param name="Name">Name used in expressions (case-insensitive).</param>
/// <param name="MinArguments">Minimum number of arguments.</param>
/// <param name="MaxArguments">Maximum number of arguments; int.MaxValue for any number.</param>
/// <param name="Invoke">
///     Implementation. Reads arguments with <see cref="FunctionArgument.Number"/> or <see cref="FunctionArgument.Text"/>,
///     which report a wrong type. Throws <see cref="ArithmeticException"/> for invalid input.
/// </param>
public record FunctionDefinition(
    string Name,
    int MinArguments,
    int MaxArguments,
    Func<IReadOnlyList<FunctionArgument>, decimal> Invoke)
{
    public bool AcceptsArgumentCount(int count) => count >= MinArguments && count <= MaxArguments;

    /// <summary>E.g. "1 argument", "1 to 2 arguments", "at least 1 argument".</summary>
    public string DescribeArgumentCount()
    {
        if (MaxArguments == int.MaxValue)
        {
            return $"at least {Plural(MinArguments)}";
        }

        return MinArguments == MaxArguments
            ? Plural(MinArguments)
            : $"{MinArguments} to {Plural(MaxArguments)}";
    }

    private static string Plural(int count) => count == 1 ? "1 argument" : $"{count} arguments";
}
