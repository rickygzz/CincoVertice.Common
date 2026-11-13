using CincoVertice.Common.Utils.Config;
using CincoVertice.Common.Utils.Config.Models;
using CincoVertice.Common.Utils.Config.Tokens;
using Xunit;

namespace CincoVertice.Common.Utils.Tests.Config.Tokens;

public class GetKeyValueTokensTest
{
    [Theory]
    [InlineData("keyFloatF = 1.", 9, 2, 14, '\0')]
    [InlineData("keyFloatF   =  1.    ", 9, 2, 21, '\0')]
    public void GetKeyValueTokens_FindsComment_ChangesCharIndex(
        string testStr,
        int expectedKeyLength,
        int expectedValueLength,
        int expectedNewIndex,
        char expectedChar)
    {
        // Arrange
        ConfigLexer lexer = new(testStr);

        // Act
        lexer.Char(0);
        ConfigItemModel ret = lexer.GetKeyValueTokens();

        // Assert
        Assert.Equal(2, lexer.Tokens.Count);
        Assert.Equal(ConfigTokenType.Key, lexer.Tokens.Token(0).Type);
        Assert.Equal(ConfigTokenType.Decimal, lexer.Tokens.Token(1).Type);

        Assert.Equal(expectedKeyLength, ret.Key.Length);
        Assert.Equal(ConfigTokenType.Key, ret.Key.Type);

        Assert.Equal(expectedValueLength, ret.Value.Length);
        Assert.Equal(ConfigTokenType.Decimal, ret.Value.Type);

        Assert.Equal(expectedNewIndex, lexer.CharIndex);
        Assert.Equal(expectedChar, lexer.CurrentChar);
    }

    [Theory]
    [InlineData("first =\nsecond = 2")]
    [InlineData("first =   \r\nsecond = 2\r\n")]
    [InlineData("first =\n\n# comment\nsecond = 2")]
    public void KeyWithoutValue_DoesNotTakeTheNextLine(string text)
    {
        // Act
        ConfigItems items = new ConfigLexer(text).Items;

        // Assert
        Assert.Equal(2, items.KeysCount);
        Assert.Equal("first", items.GetKeyName(0));
        Assert.Equal(string.Empty, items.GetValue(0));
        Assert.Equal(ConfigTokenType.ErrorNoValueFound, items.GetValueType(0));
        Assert.Equal("second", items.GetKeyName(1));
        Assert.Equal("2", items.GetValue(1));
    }

    [Theory]
    [InlineData("123Search = a", "123Search")]
    [InlineData("_private = a", "_private")]
    [InlineData("123 = a", "123")]
    [InlineData("a_1 = a", "a_1")]
    public void Key_CanStartWithADigitOrUnderscore(string text, string expectedKey)
    {
        // Act
        ConfigItems items = new ConfigLexer(text).Items;

        // Assert
        Assert.Equal(expectedKey, items.GetKeyName(0));
        Assert.Equal("a", items.GetValue(0));
    }

    [Theory]
    [InlineData("-key = a")]
    [InlineData("= a")]
    public void Key_MustStartWithALetterDigitOrUnderscore(string text)
    {
        Assert.Equal(0, new ConfigLexer(text).Items.KeysCount);
    }
}
