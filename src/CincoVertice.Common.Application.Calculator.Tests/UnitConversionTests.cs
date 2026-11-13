using CincoVertice.Common.Application.Calculator.Conversions;
using Xunit;

namespace CincoVertice.Common.Application.Calculator.Tests;

public class UnitConversionTests
{
    private readonly ExpressionCalculator _calculator = new();

    [Theory]
    // Length
    [InlineData("1 km to m", "1000 m")]
    [InlineData("1 mi to km", "1.609344 km")]
    [InlineData("1 km to mi", "0.6213711922 mi")]
    [InlineData("1 ft to in", "12 in")]
    [InlineData("1 yd to ft", "3 ft")]
    [InlineData("1 nmi to m", "1852 m")]
    [InlineData("1 mm to nm", "1000000 nm")]
    [InlineData("1 nm to km", "0.000000000001 km")]
    // Mass
    [InlineData("1 kg to g", "1000 g")]
    [InlineData("1 lb to oz", "16 oz")]
    [InlineData("1 st to lb", "14 lb")]
    [InlineData("1 ton to lb", "2000 lb")]
    [InlineData("1 t to kg", "1000 kg")]
    [InlineData("1 kg to lb", "2.204622622 lb")]
    // Temperature
    [InlineData("100 C to F", "212 F")]
    [InlineData("0 C to F", "32 F")]
    [InlineData("-40 C to F", "-40 F")]
    [InlineData("98.6 F to C", "37 C")]
    [InlineData("32 F to C", "0 C")]
    [InlineData("0 C to K", "273.15 K")]
    [InlineData("0 K to C", "-273.15 C")]
    [InlineData("212 F to K", "373.15 K")]
    // Area
    [InlineData("1 km2 to m2", "1000000 m2")]
    [InlineData("1 ha to m2", "10000 m2")]
    [InlineData("1 ft2 to in2", "144 in2")]
    [InlineData("1 mi2 to ac", "640 ac")]
    [InlineData("1 m^2 to cm^2", "10000 cm2")]
    // Volume
    [InlineData("1 L to mL", "1000 mL")]
    [InlineData("1 m3 to L", "1000 L")]
    [InlineData("1 cc to mL", "1 mL")]
    [InlineData("1 gal to qt", "4 qt")]
    [InlineData("1 qt to pt", "2 pt")]
    [InlineData("1 pt to cup", "2 cup")]
    [InlineData("1 cup to floz", "8 floz")]
    [InlineData("1 floz to tbsp", "2 tbsp")]
    [InlineData("1 tbsp to tsp", "3 tsp")]
    [InlineData("1 gal to in3", "231 in3")]
    [InlineData("1 ft3 to in3", "1728 in3")]
    [InlineData("1 ukgal to L", "4.54609 L")]
    // Time
    [InlineData("1 h to min", "60 min")]
    [InlineData("1 d to h", "24 h")]
    [InlineData("1 wk to d", "7 d")]
    [InlineData("1 yr to d", "365.25 d")]
    [InlineData("1 s to ms", "1000 ms")]
    [InlineData("3600 s to h", "1 h")]
    // Speed
    [InlineData("36 km/h to m/s", "10 m/s")]
    [InlineData("100 km/h to mph", "62.13711922 mph")]
    [InlineData("60 mph to km/h", "96.56064 km/h")]
    [InlineData("1 knot to km/h", "1.852 km/h")]
    [InlineData("1 mph to ft/s", "1.466666667 ft/s")]
    // Data
    [InlineData("1 B to b", "8 b")]
    [InlineData("1 MB to kB", "1000 kB")]
    [InlineData("1 GB to MB", "1000 MB")]
    [InlineData("1 TB to GB", "1000 GB")]
    [InlineData("1 PB to TB", "1000 TB")]
    [InlineData("1 KiB to B", "1024 B")]
    [InlineData("1 GiB to MiB", "1024 MiB")]
    [InlineData("1 TiB to GiB", "1024 GiB")]
    [InlineData("1 PiB to TiB", "1024 TiB")]
    [InlineData("1 GiB to GB", "1.073741824 GB")]
    [InlineData("100 Mb to MB", "12.5 MB")]
    [InlineData("8 Gb to GB", "1 GB")]
    [InlineData("8 Tb to TB", "1 TB")]
    [InlineData("8 kb to kB", "1 kB")]
    // The value can be any expression
    [InlineData("(1+1)*5 km to m", "10000 m")]
    [InlineData("sqrt(16) ft to in", "48 in")]
    [InlineData("1 km to km", "1 km")]
    [InlineData("0 km to mi", "0 mi")]
    [InlineData("-1 km to m", "-1000 m")]
    public void EvaluateToText_Converts(string expression, string expected)
    {
        Assert.Equal(expected, _calculator.EvaluateToText(expression));
    }

    [Theory]
    [InlineData("1 meters to feet", "3.280839895 ft")]
    [InlineData("1 kilometre to miles", "0.6213711922 mi")]
    [InlineData("1 hour to seconds", "3600 s")]
    [InlineData("1 kilo to pounds", "2.204622622 lb")]
    [InlineData("100 celsius to fahrenheit", "212 F")]
    [InlineData("1 gallon to liters", "3.785411784 L")]
    [InlineData("1 hectare to acres", "2.471053815 ac")]
    [InlineData("1 kmh to kph", "1 km/h")]
    [InlineData("1 byte to bits", "8 b")]
    [InlineData("1 kilobyte to bytes", "1000 B")]
    public void EvaluateToText_TakesLongNames_AndShowsTheSymbol(string expression, string expected)
    {
        Assert.Equal(expected, _calculator.EvaluateToText(expression));
    }

    [Theory]
    [InlineData("1 KM to M", "1000 m")]
    [InlineData("1 Km TO Mm", "1000000 mm")]
    [InlineData("100 c to f", "212 F")]
    [InlineData("1 l to ml", "1000 mL")]
    [InlineData("1 HOUR to MIN", "60 min")]
    [InlineData("1 KB to B", "1000 B")]
    public void EvaluateToText_WhenTheCaseFitsOneUnit_TakesIt(string expression, string expected)
    {
        Assert.Equal(expected, _calculator.EvaluateToText(expression));
    }

    [Theory]
    [InlineData("1 mb to kB", "Unit 'mb' could be", 2)]
    [InlineData("1 kB to gb", "Unit 'gb' could be", 8)]
    [InlineData("1 tb to kB", "Unit 'tb' could be", 2)]
    [InlineData("1 xyz to m", "Unknown unit 'xyz'", 2)]
    [InlineData("1 m to xyz", "Unknown unit 'xyz'", 7)]
    [InlineData("1 km to kg", "Can't convert km to kg", 5)]
    [InlineData("1 C to km", "Can't convert C to km", 4)]
    [InlineData("1/0 km to m", "Division by zero", 1)]
    public void EvaluateToText_WhenItCantConvert_ReportsWhereAndWhy(
        string expression,
        string expectedMessageStart,
        int expectedPosition)
    {
        // Act
        var exception = Assert.Throws<ExpressionException>(() => _calculator.EvaluateToText(expression));

        // Assert
        Assert.StartsWith(expectedMessageStart, exception.Message);
        Assert.Equal(expectedPosition, exception.Position);
    }

    [Fact]
    public void EvaluateToText_WhenTheResultIsTooLarge_ReportsIt()
    {
        // Act
        var exception = Assert.Throws<ExpressionException>(
            () => _calculator.EvaluateToText("79228162514264337593543950335 km to nm"));

        // Assert
        Assert.StartsWith("Number is too large", exception.Message);
    }

    [Theory]
    [InlineData("1+1", "2")]
    [InlineData("1234567.5", "1234567.5")]
    public void EvaluateToText_WithoutAConversion_IsTheFormattedResult(string expression, string expected)
    {
        Assert.Equal(expected, _calculator.EvaluateToText(expression));
    }

    [Fact]
    public void EvaluateToText_GroupsThousands_BeforeTheUnit()
    {
        Assert.Equal("1,000,000 m", _calculator.EvaluateToText("1000 km to m", groupThousands: true));
    }

    [Fact]
    public void Evaluate_OfAConversion_IsTheNumber()
    {
        Assert.Equal(1000m, _calculator.Evaluate("1 km to m"));
    }

    [Fact]
    public void UnitTable_EveryNameIsUnique_AndEveryUnitCanBeFoundBySymbol()
    {
        // The table's own static initializer would throw on a repeated name
        foreach (UnitDefinition unit in UnitTable.Units)
        {
            Assert.Same(unit, UnitTable.Find(unit.Symbol, 0));
        }
    }

    [Theory]
    [InlineData(UnitGroup.Length)]
    [InlineData(UnitGroup.Mass)]
    [InlineData(UnitGroup.Temperature)]
    [InlineData(UnitGroup.Area)]
    [InlineData(UnitGroup.Volume)]
    [InlineData(UnitGroup.Time)]
    [InlineData(UnitGroup.Speed)]
    [InlineData(UnitGroup.Data)]
    public void UnitTable_ConvertingThereAndBack_GivesTheSameValue(UnitGroup group)
    {
        foreach (UnitDefinition from in UnitTable.Units.Where(unit => unit.Group == group))
        {
            foreach (UnitDefinition to in UnitTable.Units.Where(unit => unit.Group == group))
            {
                decimal there = to.FromBase(from.ToBase(12.5m));
                decimal back = from.FromBase(to.ToBase(there));

                Assert.Equal(12.5m, DecimalMath.RoundToSignificantDigits(back, 10));
            }
        }
    }

    [Theory]
    [InlineData("1234.5678", 6, "1234.57")]
    [InlineData("0.00012345", 2, "0.00012")]
    [InlineData("-1234.5678", 6, "-1234.57")]
    [InlineData("123456", 2, "123456")]
    [InlineData("0.5", 1, "0.5")]
    [InlineData("0", 5, "0")]
    [InlineData("0.0000000000000000000000000001", 10, "0.0000000000000000000000000001")]
    [InlineData("99.99", 2, "100")]
    public void RoundToSignificantDigits_KeepsThatManyDigits(string value, int digits, string expected)
    {
        // Act
        decimal result = DecimalMath.RoundToSignificantDigits(
            decimal.Parse(value, System.Globalization.CultureInfo.InvariantCulture),
            digits);

        // Assert
        Assert.Equal(expected, ExpressionCalculator.Format(result));
    }
}
