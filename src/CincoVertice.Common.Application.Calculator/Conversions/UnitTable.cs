namespace CincoVertice.Common.Application.Calculator.Conversions;

/// <summary>
///     The units available in conversions (10 km to mi).
///     <para>
///         A name matches exactly first, because case can matter: MB is megabytes and Mb is megabits. Otherwise any
///         case works, as long as it only fits one unit: KM is km, but mb could be either of those two.
///     </para>
///     <para>
///         Base units: meter, kilogram, kelvin, square meter, liter, second, km/h and byte. They were chosen so that
///         the factors are exact wherever possible.
///     </para>
/// </summary>
public static class UnitTable
{
    public static readonly IReadOnlyList<UnitDefinition> Units =
    [
        // Length: the inch is exactly 25.4 mm, the mile 1760 yards, the nautical mile 1852 m
        new("nm", UnitGroup.Length, 0.000000001m, ["nanometer", "nanometers", "nanometre", "nanometres"]),
        new("um", UnitGroup.Length, 0.000001m, ["micrometer", "micrometers", "micrometre", "micrometres", "micron", "microns"]),
        new("mm", UnitGroup.Length, 0.001m, ["millimeter", "millimeters", "millimetre", "millimetres"]),
        new("cm", UnitGroup.Length, 0.01m, ["centimeter", "centimeters", "centimetre", "centimetres"]),
        new("m", UnitGroup.Length, 1m, ["meter", "meters", "metre", "metres"]),
        new("km", UnitGroup.Length, 1000m, ["kilometer", "kilometers", "kilometre", "kilometres"]),
        new("in", UnitGroup.Length, 0.0254m, ["inch", "inches"]),
        new("ft", UnitGroup.Length, 0.3048m, ["foot", "feet"]),
        new("yd", UnitGroup.Length, 0.9144m, ["yard", "yards"]),
        new("mi", UnitGroup.Length, 1609.344m, ["mile", "miles"]),
        new("nmi", UnitGroup.Length, 1852m, []),

        // Mass: the pound is exactly 0.45359237 kg; ton is the US short ton (2000 lb), tonne the metric one
        new("mg", UnitGroup.Mass, 0.000001m, ["milligram", "milligrams"]),
        new("g", UnitGroup.Mass, 0.001m, ["gram", "grams"]),
        new("kg", UnitGroup.Mass, 1m, ["kilogram", "kilograms", "kilo", "kilos"]),
        new("t", UnitGroup.Mass, 1000m, ["tonne", "tonnes"]),
        new("oz", UnitGroup.Mass, 0.028349523125m, ["ounce", "ounces"]),
        new("lb", UnitGroup.Mass, 0.45359237m, ["lbs", "pound", "pounds"]),
        new("st", UnitGroup.Mass, 6.35029318m, ["stone", "stones"]),
        new("ton", UnitGroup.Mass, 907.18474m, ["tons"]),

        // Temperature
        new("K", UnitGroup.Temperature, 1m, ["kelvin"]),
        new("C", UnitGroup.Temperature, 1m, ["celsius"], Offset: 273.15m),
        new("F", UnitGroup.Temperature, 5m, ["fahrenheit"], Offset: 459.67m, Divisor: 9m),

        // Area
        new("mm2", UnitGroup.Area, 0.000001m, ["mm^2"]),
        new("cm2", UnitGroup.Area, 0.0001m, ["cm^2"]),
        new("m2", UnitGroup.Area, 1m, ["m^2"]),
        new("km2", UnitGroup.Area, 1000000m, ["km^2"]),
        new("ha", UnitGroup.Area, 10000m, ["hectare", "hectares"]),
        new("in2", UnitGroup.Area, 0.00064516m, ["in^2"]),
        new("ft2", UnitGroup.Area, 0.09290304m, ["ft^2"]),
        new("yd2", UnitGroup.Area, 0.83612736m, ["yd^2"]),
        new("ac", UnitGroup.Area, 4046.8564224m, ["acre", "acres"]),
        new("mi2", UnitGroup.Area, 2589988.110336m, ["mi^2"]),

        // Volume: gal and its parts are US (231 cubic inches); ukgal is the imperial gallon
        new("mL", UnitGroup.Volume, 0.001m, ["milliliter", "milliliters", "millilitre", "millilitres"]),
        new("cm3", UnitGroup.Volume, 0.001m, ["cm^3", "cc"]),
        new("L", UnitGroup.Volume, 1m, ["liter", "liters", "litre", "litres"]),
        new("m3", UnitGroup.Volume, 1000m, ["m^3"]),
        new("in3", UnitGroup.Volume, 0.016387064m, ["in^3"]),
        new("ft3", UnitGroup.Volume, 28.316846592m, ["ft^3"]),
        new("tsp", UnitGroup.Volume, 0.00492892159375m, ["teaspoon", "teaspoons"]),
        new("tbsp", UnitGroup.Volume, 0.01478676478125m, ["tablespoon", "tablespoons"]),
        new("floz", UnitGroup.Volume, 0.0295735295625m, []),
        new("cup", UnitGroup.Volume, 0.2365882365m, ["cups"]),
        new("pt", UnitGroup.Volume, 0.473176473m, ["pint", "pints"]),
        new("qt", UnitGroup.Volume, 0.946352946m, ["quart", "quarts"]),
        new("gal", UnitGroup.Volume, 3.785411784m, ["gallon", "gallons"]),
        new("ukgal", UnitGroup.Volume, 4.54609m, []),

        // Time: yr is the average year, 365.25 days
        new("ms", UnitGroup.Time, 0.001m, ["millisecond", "milliseconds"]),
        new("s", UnitGroup.Time, 1m, ["sec", "secs", "second", "seconds"]),
        new("min", UnitGroup.Time, 60m, ["mins", "minute", "minutes"]),
        new("h", UnitGroup.Time, 3600m, ["hr", "hrs", "hour", "hours"]),
        new("d", UnitGroup.Time, 86400m, ["day", "days"]),
        new("wk", UnitGroup.Time, 604800m, ["week", "weeks"]),
        new("yr", UnitGroup.Time, 31557600m, ["year", "years"]),

        // Speed
        new("km/h", UnitGroup.Speed, 1m, ["kmh", "kph"]),
        new("m/s", UnitGroup.Speed, 3.6m, []),
        new("mph", UnitGroup.Speed, 1.609344m, ["mi/h"]),
        new("ft/s", UnitGroup.Speed, 1.09728m, []),
        new("knot", UnitGroup.Speed, 1.852m, ["knots", "kn"]),

        // Data: kB, MB... are powers of 1000 and KiB, MiB... powers of 1024; lowercase b is bits
        new("b", UnitGroup.Data, 0.125m, ["bit", "bits"]),
        new("kb", UnitGroup.Data, 125m, ["Kb", "kbit"]),
        new("Mb", UnitGroup.Data, 125000m, ["Mbit"]),
        new("Gb", UnitGroup.Data, 125000000m, ["Gbit"]),
        new("Tb", UnitGroup.Data, 125000000000m, ["Tbit"]),
        new("B", UnitGroup.Data, 1m, ["byte", "bytes"]),
        new("kB", UnitGroup.Data, 1000m, ["KB", "kilobyte", "kilobytes"]),
        new("MB", UnitGroup.Data, 1000000m, ["megabyte", "megabytes"]),
        new("GB", UnitGroup.Data, 1000000000m, ["gigabyte", "gigabytes"]),
        new("TB", UnitGroup.Data, 1000000000000m, ["terabyte", "terabytes"]),
        new("PB", UnitGroup.Data, 1000000000000000m, ["petabyte", "petabytes"]),
        new("KiB", UnitGroup.Data, 1024m, []),
        new("MiB", UnitGroup.Data, 1048576m, []),
        new("GiB", UnitGroup.Data, 1073741824m, []),
        new("TiB", UnitGroup.Data, 1099511627776m, []),
        new("PiB", UnitGroup.Data, 1125899906842624m, []),
    ];

    private static readonly Dictionary<string, UnitDefinition> _byName = Units
        .SelectMany(unit => unit.Names.Append(unit.Symbol), (unit, name) => (unit, name))
        .ToDictionary(x => x.name, x => x.unit, StringComparer.Ordinal);

    /// <summary>
    ///     Finds a unit by any of its names.
    /// </summary>
    /// <param name="name">The name in the expression.</param>
    /// <param name="position">Where the name is in the expression, to report an error.</param>
    /// <exception cref="ExpressionException">There is no such unit, or the name's case fits more than one.</exception>
    public static UnitDefinition Find(string name, int position)
    {
        if (_byName.TryGetValue(name, out UnitDefinition? exact))
        {
            return exact;
        }

        List<UnitDefinition> matches = _byName
            .Where(x => x.Key.Equals(name, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Value)
            .Distinct()
            .ToList();

        return matches.Count switch
        {
            0 => throw new ExpressionException($"Unknown unit '{name}'", position),
            1 => matches[0],
            _ => throw new ExpressionException(
                $"Unit '{name}' could be {string.Join(" or ", matches.Select(unit => unit.Symbol))}",
                position),
        };
    }
}
