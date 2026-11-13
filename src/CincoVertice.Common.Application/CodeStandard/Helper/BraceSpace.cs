namespace CincoVertice.Common.Application.CodeStandard.Helper;

/// <summary>
///     Brace spacing shared by the brace space checker and fixer (CH0033), so both agree on where a space goes.
/// </summary>
public static class BraceSpace
{
    /// <summary>
    ///     True when a { right after <paramref name="previous"/> needs a space before it: after code, but not at the
    ///     line start or after ( or [, as in Foo([{ ... }]).
    /// </summary>
    public static bool NeedsSpaceBeforeOpening(char previous) =>
        !char.IsWhiteSpace(previous) && previous is not ('(' or '[');
}
