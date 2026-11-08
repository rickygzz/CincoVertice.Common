namespace CincoVertice.Common.Utils.Lexer;

public class GenericTokenType
{
    /// <summary>Counter used to set a value to each property.</summary>
    protected static int count = 0;

    /// <summary>Null.</summary>
    public static readonly int Null = count++;

    /// <summary>Integer ::= [-]? {Digit}+.
    /// <para>{Digit} ::= [0123456789].</para>
    /// </summary>
    public static readonly int Integer = count++;

    /// <summary>Decimal.
    /// <para>Decimal  ::= ('-'? '.' {Digit}+) | ({Integer} '.'? {Digit}*).</para>
    /// <para>Integer  ::= '-'? {Digit}+.</para>
    /// </summary>
    public static readonly int Decimal = count++;

    /// <summary>Binary.
    /// <para>Bin ::= '0b' {BinDigit}+ .</para>
    /// <para>{BinDigit} ::= [01] .</para>
    /// </summary>
    public static readonly int Binary = count++;

    /// <summary>Oct ::= '0o' {OctDigit}+ . {OctDigit} ::= [01234567] .</summary>
    public static readonly int Octal = count++;

    /// <summary>Hex ::= '0x' {HexDigit}+ . {HexDigit}   ::= {Digit} + [abcdefABCDEF] .</summary>
    public static readonly int Hex = count++;

    public static readonly int Text = count++;
    public static readonly int String = count++;

    /// <summary>Blank.</summary>
    public static readonly int Blank = count++;

    /// <summary>End of file.</summary>
    public static readonly int End = count++;
}
