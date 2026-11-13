namespace CincoVertice.Common.Utils.Highlighting;

/// <summary>What a piece of code is, which decides its color.</summary>
public enum CodeSpanKind
{
    Comment,
    Keyword,
    Text,
    Number,
    Function,

    /// <summary>The name on the left of a config line.</summary>
    Key,
}
