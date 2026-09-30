namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CIsPrintableChar
{
    /// <summary>
    ///     {Printable} ::= [0x09 0x20-0x07E 0x80-0xFFFD].
    /// <para>Includes all standard characters.</para>
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>true if currentChar is {Printable}.</returns>
    public static bool IsPrintableChar(this IGenericLexer lexer)
    {
        return lexer.CurrentChar == '\x09'
            || lexer.CurrentChar >= '\x20' && lexer.CurrentChar <= '\x7E'
            || lexer.CurrentChar >= '\x80' && lexer.CurrentChar <= '\xFFFD';

        // return (this.currentChar >= 'A' && this.currentChar <= 'Z')
        //    || (this.currentChar == '_')
        //    || (this.currentChar >= 'a' && this.currentChar <= 'z')
        //    || (this.currentChar >= '\xC0' && this.currentChar <= '\xD6')
        //    || (this.currentChar >= '\xD8' && this.currentChar <= '\xF6')
        //    || (this.currentChar >= '\xF8' && this.currentChar <= '\x2FF')
        //    || (this.currentChar >= '\x370' && this.currentChar <= '\x37D')
        //    || (this.currentChar >= '\x37F' && this.currentChar <= '\x1FFF')
        //    || (this.currentChar >= '\x200C' && this.currentChar <= '\x200D')
        //    || (this.currentChar >= '\x2070' && this.currentChar <= '\x218F')
        //    || (this.currentChar >= '\x2070' && this.currentChar <= '\x218F')
        //    || (this.currentChar >= '\x2C00' && this.currentChar <= '\x2FEF')
        //    || (this.currentChar >= '\x3001' && this.currentChar <= '\xD7FF')
        //    || (this.currentChar >= '\xF900' && this.currentChar <= '\xFDCF')
        //    || (this.currentChar >= '\xFDF0' && this.currentChar <= '\xFFFD');
    }
}
