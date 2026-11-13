using System.Diagnostics.CodeAnalysis;

namespace CincoVertice.Common.Application.Scripting.Interfaces;

/// <summary>
///     Global variables: values kept between script runs and shared by all scripts (global.set and global.get).
///     Names are case-insensitive. Values are script values: decimal, string, bool or a list of them.
/// </summary>
public interface IScriptVariableStore
{
    bool TryGet(string name, [NotNullWhen(true)] out object? value);

    void Set(string name, object value);

    /// <returns>False when there was no variable with that name.</returns>
    bool Remove(string name);

    /// <summary>The names of the variables, sorted.</summary>
    IReadOnlyList<string> GetNames();
}
