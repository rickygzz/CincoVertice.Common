namespace CincoVertice.Common.Application.Calculator.Conversions;

/// <summary>
///     A unit, defined by how a value in it becomes a value in its group's base unit:
///     (value + <paramref name="Offset"/>) * <paramref name="Factor"/> / <paramref name="Divisor"/>.
/// </summary>
/// <param name="Symbol">Name used in expressions and shown after a result.</param>
/// <param name="Group">What it measures.</param>
/// <param name="Factor">How many base units one of this unit is.</param>
/// <param name="Names">Other names used in expressions, e.g. "meters".</param>
/// <param name="Offset">Only temperatures have one, because their zeros differ.</param>
/// <param name="Divisor">
///     For a factor decimal can't hold exactly, like the 5/9 of Fahrenheit, which would turn 32 F into a tiny number
///     instead of 0 C. Multiplying before dividing keeps these results exact.
/// </param>
public sealed record UnitDefinition(
    string Symbol,
    UnitGroup Group,
    decimal Factor,
    string[] Names,
    decimal Offset = 0,
    decimal Divisor = 1)
{
    public decimal ToBase(decimal value) => (value + Offset) * Factor / Divisor;

    public decimal FromBase(decimal value) => value * Divisor / Factor - Offset;
}
