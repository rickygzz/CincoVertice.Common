using CincoVertice.Common.Application.Scripting.Functions;
using CincoVertice.Common.Application.Scripting.Interfaces;

namespace CincoVertice.Common.Application.Scripting;

/// <summary>
///     Runs a script: tokenizes and parses all of it first, so a syntax error stops it before anything happens, then
///     runs it with the built-in functions acting through <see cref="IScriptHost"/>.
/// </summary>
public class ScriptEngine
{
    /// <exception cref="ScriptException">A syntax or run-time error, with its line and column.</exception>
    public async Task RunAsync(string source, IScriptHost host)
    {
        var tokens = new ScriptLexer().Tokenize(source);
        var statements = new ScriptParser().Parse(tokens);

        await new ScriptInterpreter(BuiltInFunctions.Create(host))
            .RunAsync(statements);
    }
}
