using CincoVertice.Common.Utils.Config;
using Xunit;

namespace CincoVertice.Common.Utils.Tests.Config;

public class ConfigItemsTest
{
    public class Sample
    {
        public int Number = 7;
        public long Big = 8;
        public bool Flag = true;
        public string Name = "default";
        public DateTime Date = DateTime.UnixEpoch;
    }

    [Fact]
    public void GetValues_SetsEachFieldFromItsKey()
    {
        // Act
        Sample result = new ConfigLexer("Number = 1\nBig = 2\nFlag = false\nName = a").Items.GetValues<Sample>();

        // Assert
        Assert.Equal(1, result.Number);
        Assert.Equal(2, result.Big);
        Assert.False(result.Flag);
        Assert.Equal("a", result.Name);
    }

    [Fact]
    public void GetValues_WhenKeysAreMissing_KeepsTheInitialValues()
    {
        // Act
        Sample result = new ConfigLexer("other = 1").Items.GetValues<Sample>();

        // Assert
        Assert.Equal(7, result.Number);
        Assert.Equal(8, result.Big);
        Assert.True(result.Flag);
        Assert.Equal("default", result.Name);
    }

    [Fact]
    public void GetValues_WhenAValueCantBeConverted_KeepsTheInitialValue()
    {
        // Act
        Sample result = new ConfigLexer("Number = abc\nFlag = maybe").Items.GetValues<Sample>();

        // Assert
        Assert.Equal(7, result.Number);
        Assert.True(result.Flag);
    }

    [Fact]
    public void GetValues_WhenAFieldsTypeIsNotSupported_KeepsTheInitialValue()
    {
        Assert.Equal(DateTime.UnixEpoch, new ConfigLexer("Date = 2026-01-01").Items.GetValues<Sample>().Date);
    }
}
