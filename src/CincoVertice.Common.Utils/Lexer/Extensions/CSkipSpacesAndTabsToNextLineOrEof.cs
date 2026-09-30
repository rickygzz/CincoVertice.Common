namespace CincoVertice.Common.Utils.Lexer.Extensions;

public static class CSkipSpacesAndTabsToNextLineOrEof
{
    /// <summary>
    ///     Skips all spaces and tab, until it skips first {EOL} or {EOF}.
    /// </summary>
    /// <param name="lexer">Lexer instance.</param>
    /// <returns>true if {EOL} was skipped or it reached {EOF}.</returns>
    public static bool SkipSpacesAndTabsToNextLineOrEof(this IGenericLexer lexer)
    {
        lexer.SkipSpacesAndTabs();

        return lexer.SkipOneEOL();
    }
}
