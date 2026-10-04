using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Constants;

public static class Errors
{
    #region File Errors
    public const string CH0001 = "File not found.";

    public const string CH0002 = "Error reading file: {message}";
    #endregion

    #region
    public const string CH0010 = "Line contains whitespaces only.";

    public const string CH0011 = "Line ends with whitespace.";

    public const string CH0012 = "Line is tab indented.";

    public const string CH0013 = "Line should have four spaces per indentation level.";

    public const string CH0014 = "Additional indentation should be four spaces only.";

    public const string CH0015 = "Reducing indentation should be four spaces only.";

    public const string CH0016 = "No lines should be greater than 120 characters.";

    public const string CH0017 = "Line starts with forbidden token: '{token}'";

    public const string CH0018 = "Line ends with forbidden token: '{token}'";

    public const string CH0019 = "Line contains forbidden token: '{token}'";


    public const string CH0020 = "Character immediately following /// is not a space.";

    public const string CH0021 = "Character immediately following // is not a space.";

    public const string CH0022 = "Keyword '{keyword}' lacks space before parenthesis.";

    public const string CH0023 = "Keyword '{keyword}' has spaces before parenthesis.";

    public const string CH0024 = "Keyword '{keyword}' should have only one space before parenthesis.";

    public const string CH0025 = "Line contains invisible character {character} at column {column}.";

    public const string CH0026 = "Line contains Unicode space {character} at column {column}; use a normal space.";

    public const string CH0027 = "Line is a consecutive blank line.";

    public const string CH0028 = "Blank line before closing brace.";

    public const string CH0029 = "Namespace should be file-scoped: namespace Name;";

    public const string CH0030 = "Empty XML comments.";

    public const string CH0031 = "if / else body must be in braces.";
    #endregion

    public static ErrorModel New(int line, string code, string message)
    {
        return new ErrorModel { Line = line, Code = code, Message = message };
    }
}
