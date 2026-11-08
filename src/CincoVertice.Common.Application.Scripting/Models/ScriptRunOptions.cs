namespace CincoVertice.Common.Application.Scripting.Models;

/// <summary>
///     What a run of a script gets from where it was started.
/// </summary>
public sealed class ScriptRunOptions
{
    /// <summary>
    ///     The value of script.argument(), e.g. "app1" from "PrTemplate = Ctrl+Alt+1, app1" in hotkeys.cfg; empty
    ///     when none was given.
    /// </summary>
    public string Argument { get; init; } = string.Empty;

    /// <summary>
    ///     Reads the source of the script that script.include(name) names. Null when scripts can't include others.
    ///     Throws, with a message for the user, when there is no such script.
    /// </summary>
    public Func<string, string>? ReadInclude { get; init; }
}
