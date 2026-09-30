using CincoVertice.Common.Utils.Config;
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
}
