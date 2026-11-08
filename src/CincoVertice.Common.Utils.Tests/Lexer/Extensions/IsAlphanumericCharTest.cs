using CincoVertice.Common.Utils.Lexer;
using CincoVertice.Common.Utils.Lexer.Extensions;
using Xunit;

namespace CincoVertice.Common.Utils.Tests.Lexer.Extensions;

public class IsAlphanumericCharTest
{
    [Theory]
    [InlineData("ABCDEFGHIJKLMNOPQRSTUVWXYZ")]
    [InlineData("abcdefghijklmnopqrstuvwxyz")]
    [InlineData("áéíóúÁÉÍÓÚàèìòùÀÈÌÒÙäëïöüÄËÏÖÜâêîôûÂÊÎÔÛÇçñÑ")]
    [InlineData("ÅåÆæÐÕõØøÝýßðþÿ")]
    [InlineData("1234567890")]
    public void IsAlphanumericChar_GetsValidAlphanumericChar_ReturnsTrue(string testStr)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Assert
        while (lexer.CurrentChar != '\0')
        {
            Assert.True(lexer.IsAlphanumericChar());

            lexer.NextChar();
        }
    }

    [Theory]
    [InlineData("¨´`'\\ªº")]
    [InlineData("/()+-*/=_<>")]
    [InlineData("@#~¬|¿?&%·!^¨:;,.¡")]
    [InlineData("$¥€")]
    public void IsAlphanumericChar_GetsInvalidAlphanumericChar_ReturnsFalse(string testStr)
    {
        // Arrange
        GenericLexer lexer = new(testStr);

        // Assert
        while (lexer.CurrentChar != '\0')
        {
            Assert.False(lexer.IsAlphanumericChar());

            lexer.NextChar();
        }
    }
}
