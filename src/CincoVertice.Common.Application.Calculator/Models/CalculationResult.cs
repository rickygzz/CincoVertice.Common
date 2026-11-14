namespace CincoVertice.Common.Application.Calculator.Models;

/// <summary>
///     What <see cref="ExpressionCalculator.Calculate"/> gives.
/// </summary>
/// <param name="Text">The result as shown: 22, 6.213711922 mi, 2026-12-25 (Friday), 22:00 India (UTC+5:30).</param>
/// <param name="Value">The value, to keep as the last answer or in a name; null for a time in another place.</param>
/// <param name="AssignedName">The name it was given, as in x = 5; null when none was.</param>
public sealed record CalculationResult(string Text, CalculatorValue? Value, string? AssignedName = null);
