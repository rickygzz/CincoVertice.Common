using CincoVertice.Common.Utils.Config;
using CincoVertice.Common.Utils.Config.Models;
using Xunit;

namespace CincoVertice.Common.Utils.Tests.Config;

public class ConfigLexerTest
{
    [Theory]
    [InlineData("")]
    [InlineData("   \n\n")]
    [InlineData("# comment\n  # comment")]
    [InlineData("#\n#\r\n#")]
    [InlineData("key = value\nother = 1\n")]
    [InlineData("key =\nother =")]
    public void BadLines_WhenEveryLineIsRead_IsEmpty(string text)
    {
        Assert.Empty(new ConfigLexer(text).BadLines);
    }

    [Theory]
    [InlineData("first = 1\nCtrl+A = 2\nlast = 3")]
    [InlineData("first = 1\r\nCtrl+A = 2\r\nlast = 3\r\n")]
    [InlineData("first = 1\rCtrl+A = 2\rlast = 3")]
    [InlineData("first = 1\n   Ctrl+A = 2   \nlast = 3")]
    public void BadLines_GivesTheLineAndItsNumber(string text)
    {
        // Act
        ConfigLexer lexer = new(text);

        // Assert
        Assert.Equal(new ConfigBadLine(2, "Ctrl+A = 2"), Assert.Single(lexer.BadLines));
        Assert.Equal(2, lexer.Items.KeysCount);
        Assert.Equal("1", lexer.Items.GetValue("first"));
        Assert.Equal("3", lexer.Items.GetValue("last"));
    }

    [Fact]
    public void BadLines_CountsBlankAndCommentLines()
    {
        // Act
        ConfigLexer lexer = new("# comment\n\n\r\nno equals\n\nkey = 1\n# comment\nanother bad line");

        // Assert
        Assert.Equal(
            [
                new ConfigBadLine(4, "no equals"),
                new ConfigBadLine(8, "another bad line")
            ],
            lexer.BadLines);
    }

    [Fact]
    public void BadLines_DoesNotCountTheLinesOfAQuotedValue()
    {
        // Act
        ConfigLexer lexer = new("key = [\"line one\nline two\"]\nother = 1");

        // Assert
        Assert.Empty(lexer.BadLines);
        Assert.Equal(2, lexer.Items.KeysCount);
        Assert.Equal("line one\nline two", lexer.Items.GetValue("key"));
        Assert.Equal("1", lexer.Items.GetValue("other"));
    }

    [Fact]
    public void BadLines_DoesNotCountAKeyWithTheEqualsOnTheNextLine()
    {
        // Act
        ConfigLexer lexer = new("key\n= 1");

        // Assert
        Assert.Empty(lexer.BadLines);
        Assert.Equal("1", lexer.Items.GetValue("key"));
    }

    [Fact]
    public void BadLines_AreForTheCurrentText()
    {
        // Arrange
        ConfigLexer lexer = new("bad line");

        // Act
        lexer.Text = "key = 1\nother bad line";

        // Assert
        Assert.Equal(new ConfigBadLine(2, "other bad line"), Assert.Single(lexer.BadLines));
    }

    [Theory]
    [InlineData("Ctrl+A = 2")]
    [InlineData("-key = a")]
    [InlineData("= a")]
    [InlineData("no equals")]
    [InlineData("two words = 1")]
    [InlineData("key : 1")]
    [InlineData("[section]")]
    public void BadLines_WhenALineIsNotAKeyWithAValue_GivesTheLine(string line)
    {
        // Act
        ConfigLexer lexer = new(line);

        // Assert
        Assert.Equal(new ConfigBadLine(1, line), Assert.Single(lexer.BadLines));
        Assert.Equal(0, lexer.Items.KeysCount);
    }

    [Theory]
    [InlineData("\n\nbad", 3)]
    [InlineData("\r\n\r\nbad", 3)]
    [InlineData("\r\rbad", 3)]
    [InlineData("\n\r\nbad", 3)]
    [InlineData("key =\nbad", 2)]
    [InlineData("key = [\"a\nb\"]\nbad", 3)]
    [InlineData("key = [\"a\r\nb\"]\r\nbad", 3)]
    public void BadLines_CountsEveryKindOfLineEnd(string text, int expectedNumber)
    {
        // Act
        ConfigLexer lexer = new(text);

        // Assert
        Assert.Equal(new ConfigBadLine(expectedNumber, "bad"), Assert.Single(lexer.BadLines));
    }

    [Fact]
    public void BadLines_GivesEveryBadLineInOrder()
    {
        // Act
        ConfigLexer lexer = new("bad one\nbad two\nkey = 1\nbad three\n");

        // Assert
        Assert.Equal(
            [
                new ConfigBadLine(1, "bad one"),
                new ConfigBadLine(2, "bad two"),
                new ConfigBadLine(4, "bad three")
            ],
            lexer.BadLines);
        Assert.Equal("1", lexer.Items.GetValue("key"));
    }

    [Fact]
    public void BadLines_GivesTextAfterAQuotedValue()
    {
        // Act
        ConfigLexer lexer = new("key = [\"a\"] junk\nother = 1");

        // Assert
        Assert.Equal(new ConfigBadLine(1, "junk"), Assert.Single(lexer.BadLines));
        Assert.Equal("a", lexer.Items.GetValue("key"));
        Assert.Equal("1", lexer.Items.GetValue("other"));
    }

    [Fact]
    public void BadLines_AfterSet_AreNotRepeated()
    {
        // Arrange
        ConfigLexer lexer = new("bad line\nkey = 1");

        // Act
        lexer.Items.Set("key", "2");
        lexer.Items.Set("added", "3");

        // Assert
        Assert.Equal(new ConfigBadLine(1, "bad line"), Assert.Single(lexer.BadLines));
        Assert.Equal("2", lexer.Items.GetValue("key"));
        Assert.Equal("3", lexer.Items.GetValue("added"));
    }

    [Fact]
    public void Constructor_WithoutText_HasNothingToRead()
    {
        // Act
        ConfigLexer lexer = new();

        // Assert
        Assert.Empty(lexer.BadLines);
        Assert.Equal(0, lexer.Items.KeysCount);
        Assert.Equal(0, lexer.Tokens.Count);
    }

    [Fact]
    public void Tokens_HaveCommentsKeysAndValuesInOrder_AndNoBadLines()
    {
        // Act
        ConfigLexer lexer = new("# comment\nkey = 1\nbad line\nother = a");
        int[] types = [.. Enumerable.Range(0, lexer.Tokens.Count).Select(i => lexer.Tokens.Token(i).Type)];

        // Assert
        int[] expected =
        [
            ConfigTokenType.Comment,
            ConfigTokenType.Key,
            ConfigTokenType.Integer,
            ConfigTokenType.Key,
            ConfigTokenType.StringUnquoted
        ];
        Assert.Equal(expected, types);
    }

    [Theory]
    [InlineData("# key = 1")]
    [InlineData("   # key = 1")]
    [InlineData("#key = 1")]
    public void Items_LeaveOutACommentedKey(string text)
    {
        // Act
        ConfigLexer lexer = new(text);

        // Assert
        Assert.Equal(0, lexer.Items.KeysCount);
        Assert.Empty(lexer.BadLines);
    }

    [Theory]
    [InlineData("key = 1 # note", "1 # note")]
    [InlineData("key = a # note", "a # note")]
    [InlineData("key = Ctrl+# # note", "Ctrl+# # note")]
    public void Items_KeepACommentAfterAValueInTheValue(string text, string expectedValue)
    {
        Assert.Equal(expectedValue, new ConfigLexer(text).Items.GetValue("key"));
    }

    [Fact]
    public void Items_KeepARepeatedKey_AndGetValueGivesTheFirst()
    {
        // Act
        ConfigItems items = new ConfigLexer("key = 1\nKEY = 2").Items;

        // Assert
        Assert.Equal(2, items.KeysCount);
        Assert.Equal("KEY", items.GetKeyName(1));
        Assert.Equal("2", items.GetValue(1));
        Assert.Equal("1", items.GetValue("key"));
    }

    [Fact]
    public void Items_WhenAQuoteIsNotClosed_TakesTheRestOfTheText()
    {
        // Act
        ConfigLexer lexer = new("key = [\"a\nother = 1");

        // Assert
        Assert.Equal(1, lexer.Items.KeysCount);
        Assert.Equal(ConfigTokenType.ErrorQuotedStringNotClosed, lexer.Items.GetValueType(0));
        Assert.Equal(string.Empty, lexer.Items.GetValue("key"));
        Assert.Empty(lexer.BadLines);
    }
}
