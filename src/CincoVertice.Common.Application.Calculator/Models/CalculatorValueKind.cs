namespace CincoVertice.Common.Application.Calculator.Models;

/// <summary>
///     What a <see cref="CalculatorValue"/> is.
/// </summary>
public enum CalculatorValueKind
{
    Number,
    Date,

    /// <summary>A length of time, such as 30 days, 2 weeks or 3 months, added to or taken from a date.</summary>
    Duration,
}
