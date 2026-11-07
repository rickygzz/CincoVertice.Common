using CincoVertice.Common.Application.Calculator.TimeZones;
using Xunit;

namespace CincoVertice.Common.Application.Calculator.Tests;

public class TimeZoneConversionTests
{
    private const string MexicoCity = "America/Mexico_City";

    // 10:00 in Mexico City; Miami is on standard time (UTC-5)
    private static readonly DateTimeOffset _winter = new(2026, 1, 15, 16, 0, 0, TimeSpan.Zero);

    // 10:00 in Mexico City, which has no daylight saving time; Miami is on daylight saving time (UTC-4)
    private static readonly DateTimeOffset _summer = new(2026, 7, 15, 16, 0, 0, TimeSpan.Zero);

    private static string Convert(string expression, DateTimeOffset? utcNow = null) =>
        new ExpressionCalculator(new FixedClock(utcNow ?? _winter, MexicoCity)).EvaluateToText(expression);

    [Theory]
    [InlineData("10:00 Mexico to Miami", "11:00 Miami (UTC-5)")]
    [InlineData("10:00 Mexico to Cali", "11:00 Cali (UTC-5)")]
    [InlineData("10:00 mexico to bogota", "11:00 Bogotá (UTC-5)")]
    [InlineData("10:00 Mexico to Sao Paulo", "13:00 São Paulo (UTC-3)")]
    [InlineData("10:00 Mexico to India", "21:30 India (UTC+5:30)")]
    [InlineData("10:00 Mexico to China", "0:00 China (UTC+8), next day")]
    [InlineData("11:00 Miami to Mexico", "10:00 Mexico City (UTC-6)")]
    [InlineData("21:30 india to cdmx", "10:00 Mexico City (UTC-6)")]
    [InlineData("8:00 China to Bogota", "19:00 Bogotá (UTC-5), previous day")]
    public void EvaluateToText_ConvertsATimeBetweenPlaces(string expression, string expected)
    {
        Assert.Equal(expected, Convert(expression));
    }

    [Fact]
    public void EvaluateToText_FollowsDaylightSavingTime()
    {
        // Act: Miami moves its clocks, Mexico City no longer does
        string winter = Convert("10:00 Mexico to Miami", _winter);
        string summer = Convert("10:00 Mexico to Miami", _summer);

        // Assert
        Assert.Equal("11:00 Miami (UTC-5)", winter);
        Assert.Equal("12:00 Miami (UTC-4)", summer);
    }

    [Theory]
    [InlineData("3 pm Mexico to Miami", "4:00 pm Miami (UTC-5)")]
    [InlineData("3:30 PM mexico to utc", "9:30 pm UTC")]
    [InlineData("12 am Mexico to UTC", "6:00 am UTC")]
    [InlineData("12 pm Mexico to UTC", "6:00 pm UTC")]
    [InlineData("11:30 pm Mexico to UTC", "5:30 am UTC, next day")]
    public void EvaluateToText_WithAmOrPm_AnswersTheSameWay(string expression, string expected)
    {
        Assert.Equal(expected, Convert(expression));
    }

    [Theory]
    [InlineData("10:00 to India", "21:30 India (UTC+5:30)")]
    [InlineData("10:00 local to UTC", "16:00 UTC")]
    [InlineData("now to India", "21:30 India (UTC+5:30)")]
    [InlineData("now Miami to China", "0:00 China (UTC+8), next day")]
    [InlineData("NOW to local", "10:00 local (UTC-6)")]
    public void EvaluateToText_WithoutAPlace_UsesTheLocalTimeZone(string expression, string expected)
    {
        Assert.Equal(expected, Convert(expression));
    }

    [Theory]
    [InlineData("10:00 sao_paulo to India", "18:30 India (UTC+5:30)")]
    [InlineData("10:00 America/Lima to Mexico", "9:00 Mexico City (UTC-6)")]
    [InlineData("10:00 Mexico to Asia/Tokyo", "1:00 Asia/Tokyo (UTC+9), next day")]
    [InlineData("10:00 Mexico to utc-3", "13:00 UTC-3")]
    [InlineData("10:00 utc+5:30 to GMT", "4:30 UTC")]
    [InlineData("10:00 UTC to gmt-3:30", "6:30 UTC-3:30")]
    public void EvaluateToText_TakesTimeZoneIdsAndUtcOffsets(string expression, string expected)
    {
        Assert.Equal(expected, Convert(expression));
    }

    [Fact]
    public void EvaluateToText_ATimeSkippedByTheClocks_IsRefused()
    {
        // Arrange: on 8 March 2026, Miami's clocks went from 2:00 to 3:00
        var springForward = new DateTimeOffset(2026, 3, 8, 12, 0, 0, TimeSpan.Zero);

        // Act
        var exception = Assert.Throws<ExpressionException>(() => Convert("2:30 Miami to UTC", springForward));

        // Assert
        Assert.StartsWith("2:30 doesn't exist in Miami today", exception.Message);
        Assert.Equal(0, exception.Position);
    }

    [Fact]
    public void EvaluateToText_ATimeThatHappensTwice_IsTakenInStandardTime()
    {
        // Arrange: on 1 November 2026, Miami's clocks went from 2:00 back to 1:00
        var fallBack = new DateTimeOffset(2026, 11, 1, 12, 0, 0, TimeSpan.Zero);

        // Act
        string text = Convert("1:30 Miami to UTC", fallBack);

        // Assert
        Assert.Equal("6:30 UTC", text);
    }

    [Theory]
    [InlineData("10:00", "Expected a time zone or 'to' after the time", 5)]
    [InlineData("10:00 Mexico", "Expected 'to' after 'Mexico'", 12)]
    [InlineData("10:00 Mexico 5 to India", "Expected 'to' after 'Mexico'", 13)]
    [InlineData("10:00 to", "Expected a time zone after 'to'", 8)]
    [InlineData("10:00 to India 5", "Unexpected '5'", 15)]
    [InlineData("10: to UTC", "Expected minutes after ':'", 3)]
    [InlineData("24:00 to UTC", "The hour must be from 0 to 23", 0)]
    [InlineData("13 pm to UTC", "With am or pm, the hour must be from 1 to 12", 0)]
    [InlineData("0:30 am to UTC", "With am or pm, the hour must be from 1 to 12", 0)]
    [InlineData("10:5 to UTC", "The minutes must be two digits, from 00 to 59", 3)]
    [InlineData("10:60 to UTC", "The minutes must be two digits, from 00 to 59", 3)]
    [InlineData("1.5 pm to UTC", "Write a time as 10:30 or 10:30 pm", 0)]
    [InlineData("10:00 Narnia to UTC", "Unknown time zone 'Narnia'", 6)]
    [InlineData("10:00 to new narnia", "Unknown time zone 'new narnia'", 9)]
    [InlineData("10:00 to UTC+15", "A UTC offset must be from UTC-14 to UTC+14", 9)]
    [InlineData("10:00 to UTC+5:75", "A UTC offset must be from UTC-14 to UTC+14", 9)]
    public void EvaluateToText_WhenItCantConvert_ReportsWhereAndWhy(string expression, string message, int position)
    {
        // Act
        var exception = Assert.Throws<ExpressionException>(() => Convert(expression));

        // Assert
        Assert.Equal($"{message} at position {position + 1}", exception.Message);
    }

    [Theory]
    [InlineData("3 h to min", "180 min")]
    [InlineData("2 + 3", "5")]
    public void EvaluateToText_ANumberWithoutAmOrPm_IsStillANumber(string expression, string expected)
    {
        Assert.Equal(expected, Convert(expression));
    }

    [Fact]
    public void Evaluate_ATime_HasNoNumberValue()
    {
        // Act
        var exception = Assert.Throws<ExpressionException>(() => new ExpressionCalculator().Evaluate("10:00 to UTC"));

        // Assert
        Assert.Contains("A time has no number value", exception.Message);
    }

    [Fact]
    public void Places_AreAllKnownToTheSystem_AndNoNameIsUsedTwice()
    {
        // Act: the table fails to load if a name is used twice
        TimeZoneInfo local = TimeZoneInfo.Utc;
        var found = TimeZoneTable.Places.Select(place => TimeZoneTable.Find(place.Name, 0, local)).ToList();

        // Assert
        Assert.Equal(TimeZoneTable.Places.Select(place => place.Name), found.Select(x => x.Name));
    }
}
