using System.Diagnostics.CodeAnalysis;
using CincoVertice.Common.Application.Scripting.Interfaces;

namespace CincoVertice.Common.Application.Scripting.Services;

/// <summary>
///     Global variables kept in memory, e.g. for tests: they last as long as the store.
/// </summary>
public sealed class MemoryVariableStore : IScriptVariableStore
{
    private readonly Dictionary<string, object> _variables = new(StringComparer.OrdinalIgnoreCase);

    public bool TryGet(string name, [NotNullWhen(true)] out object? value) =>
        _variables.TryGetValue(name, out value);

    public void Set(string name, object value)
    {
        // Removed first, so the name keeps the spelling of the last set
        _variables.Remove(name);
        _variables[name] = value;
    }

    public bool Remove(string name) => _variables.Remove(name);

    public IReadOnlyList<string> GetNames() => [.. _variables.Keys.Order(StringComparer.OrdinalIgnoreCase)];
}
