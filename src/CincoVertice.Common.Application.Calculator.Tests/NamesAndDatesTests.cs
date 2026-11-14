using CincoVertice.Common.Application.Calculator.Models;
using Xunit;

namespace CincoVertice.Common.Application.Calculator.Tests;

public class NamesAndDatesTests
{
    // Friday 13 November 2026
    private readonly ExpressionCalculator _calculator =
        new(new FixedClock(new DateTimeOffset(2026, 11, 13, 12, 0, 0, TimeSpan.Zero), "UTC"));

    private static Dictionary<string, CalculatorValue> Names(params (string Name, decimal Value)[] names) =>
        names.ToDictionary(name => name.Name, name => CalculatorValue.FromNumber(name.Value));

    [Theory]
    [InlineData("today", "2026-11-13 (Friday)")]
    [InlineData("today + 30 d", "2026-12-13 (Sunday)")]
    [InlineData("today + 30", "2026-12-13 (Sunday)")]
    [InlineData("30 days + today", "2026-12-13 (Sunday)")]
    [InlineData("today - 2 wk", "2026-10-30 (Friday)")]
    [InlineData("today + 1 yr", "2027-11-13 (Saturday)")]
    [InlineData("2026-01-31 + 1 month", "2026-02-28 (Saturday)")]
    [InlineData("2026-1-5", "2026-01-05 (Monday)")]
    [InlineData("2026-12-25 - today", "42 days")]
    [InlineData("today - 2026-12-25", "-42 days")]
    [InlineData("2026-12-25 - 2026-12-25", "0 days")]
    [InlineData("2 wk + 3 d", "17 days")]
    [InlineData("1 yr + 2 mo", "14 months")]
    [InlineData("2 * 1 wk", "14 days")]
    [InlineData("1 month", "1 month")]
    [InlineData("-1 d", "-1 day")]
    [InlineData("(2026-12-25 - today) / 2", "21 days")]
    public void Dates_MoveByLengthsOfTime_AndAreALengthOfTimeApart(string expression, string expected)
    {
        // Act / Assert
        Assert.Equal(expected, _calculator.Calculate(expression).Text);
    }

    [Theory]
    [InlineData("1 yr to d", "365.25 d")]
    [InlineData("1 d to h", "24 h")]
    [InlineData("1 wk to h", "168 h")]
    [InlineData("2026 - 12 - 25", "1989")]
    public void UnitsAndNumbers_StayAsTheyWere(string expression, string expected)
    {
        // Act / Assert: before 'to', a unit of time is a conversion's; spaced out, a date is a sum
        Assert.Equal(expected, _calculator.Calculate(expression).Text);
    }

    [Theory]
    [InlineData("today * 2", "Can't multiply a date and a number", 6)]
    [InlineData("today + today", "Can't add a date and a date", 6)]
    [InlineData("today + 1.5 d", "A date moves by whole days and whole months", 6)]
    [InlineData("2026-02-30", "There's no such date as 2026-02-30", 0)]
    [InlineData("sqrt(today)", "Expected a number, not a date", 5)]
    [InlineData("-today", "A date can't be negative", 0)]
    public void Dates_ThatCantBeCalculated_SayWhy(string expression, string message, int position)
    {
        // Act
        var error = Assert.Throws<ExpressionException>(() => _calculator.Calculate(expression));

        // Assert
        Assert.Equal((message, position), (error.Message.Split(" at position")[0], error.Position));
    }

    [Fact]
    public void Assignment_GivesTheValue_AndTheName()
    {
        // Act
        CalculationResult result = _calculator.Calculate("Rate = 5 km to m");

        // Assert: the value is the conversion's
        Assert.Equal("5000 m", result.Text);
        Assert.Equal(CalculatorValue.FromNumber(5000), result.Value);
        Assert.Equal("Rate", result.AssignedName);
    }

    [Theory]
    [InlineData("x * 2", "10")]
    [InlineData("X * 2", "10")]
    [InlineData("ans + x", "8")]
    [InlineData("y = ans * x", "15")]
    public void Names_GivenEarlier_AreUsed_InAnyLetterCase(string expression, string expected)
    {
        // Act / Assert
        Assert.Equal(expected, _calculator.Calculate(expression, Names(("x", 5), ("ans", 3))).Text);
    }

    [Fact]
    public void Names_CanKeepADate()
    {
        // Arrange
        Dictionary<string, CalculatorValue> names = new()
        {
            ["christmas"] = _calculator.Calculate("christmas = 2026-12-25").Value!,
        };

        // Act / Assert
        Assert.Equal("42 days", _calculator.Calculate("christmas - today", names).Text);
    }

    [Theory]
    [InlineData("pi = 3", "'pi' is a constant, so it can't be given a value")]
    [InlineData("sqrt = 2", "'sqrt' is a function, so it can't be given a value")]
    [InlineData("ANS = 2", "'ANS' is the last answer, so it can't be given a value")]
    [InlineData("today = 2", "'today' is today's date, so it can't be given a value")]
    [InlineData("to = 2", "'to' is a word the calculator uses, so it can't be given a value")]
    [InlineData("x =", "Expected a value after '='")]
    [InlineData("x = 10:30 Mexico to India", "A time in another place can't be kept in a name")]
    [InlineData("ans * 2", "There's no last answer yet")]
    [InlineData("x * 2", "Unknown name 'x'")]
    [InlineData("5 = 3", "Unexpected '='")]
    public void Names_ThatCantBeUsed_SayWhy(string expression, string message)
    {
        // Act
        var error = Assert.Throws<ExpressionException>(() => _calculator.Calculate(expression));

        // Assert
        Assert.StartsWith(message, error.Message);
    }

    [Fact]
    public void ATimeInAnotherPlace_HasNoValueToKeep()
    {
        // Act
        CalculationResult result = _calculator.Calculate("10:00 to UTC");

        // Assert
        Assert.Null(result.Value);
        Assert.Null(result.AssignedName);
    }
}
