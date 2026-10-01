using CincoVertice.Common.Utils.Lexer;

namespace CincoVertice.Common.Utils.Config;

/// <summary>
///     Key, value types and errors.
/// </summary>
public class ConfigTokenType : GenericTokenType
{
    // private static int count = End + 1;

    /// <summary>Null.</summary>
    public static readonly int Key = count++;

    public static readonly int StringUnquoted = count++;

    public static readonly int StringQuoted = count++;

    public static readonly int Comment = count++;

    public static readonly int ErrorUnknown = count++;

    /// <summary>
    /// ErrorCommentEnd: It starts as a comment (with #) but it does not end in {EOL}
    /// </summary>
    public static readonly int ErrorCommentEnd = count++;

    /// <summary>
    ///     ErrorKeyHasNoEqual: It starts with a key name but is not followed by a =.
    /// </summary>
    // ErrorKeyHasNoEqual,

    public static readonly int ErrorHexDigitAfter0x = count++;

    public static readonly int ErrorBinDigitAfter0b = count++;

    public static readonly int ErrorNoDigitsAfterDecimalSeparator = count++;

    public static readonly int ErrorNoValueFound = count++;

    public static readonly int ErrorQuotedStringNotClosed = count++;
}
