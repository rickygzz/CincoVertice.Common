using CincoVertice.Common.Application.Scripting.Ast;
using CincoVertice.Common.Application.Scripting.Functions;
using CincoVertice.Common.Application.Scripting.Interfaces;
using CincoVertice.Common.Application.Scripting.Models;
using CincoVertice.Common.Application.Scripting.Services;

namespace CincoVertice.Common.Application.Scripting;

/// <summary>
///     Runs a script: tokenizes and parses all of it first, so a syntax error stops it before anything happens, then
///     runs it with the built-in functions acting through <see cref="IScriptHost"/>.
/// </summary>
public class ScriptEngine
{
    // Deeper is a script that includes itself, directly or through others
    private const int MaxIncludeDepth = 10;

    private readonly IScriptVariableStore _globals;

    /// <summary>
    ///     An engine whose global variables last as long as it does.
    /// </summary>
    public ScriptEngine()
        : this(new MemoryVariableStore())
    {
    }

    /// <param name="globals">Where global.set and global.get keep their values, e.g. a file.</param>
    public ScriptEngine(IScriptVariableStore globals)
    {
        _globals = globals;
    }

    /// <param name="source">The script.</param>
    /// <param name="host">What the script does on the desktop.</param>
    /// <param name="options">The script's argument and how it includes other scripts; none by default.</param>
    /// <exception cref="ScriptException">A syntax or run-time error, with its line and column.</exception>
    public async Task RunAsync(string source, IScriptHost host, ScriptRunOptions? options = null)
    {
        options ??= new ScriptRunOptions();
        var statements = Parse(source);
        ScriptInterpreter? interpreter = null;
        int includeDepth = 0;

        // script.include(name): parsed when it runs, then run with the variables of the script that includes it
        async Task IncludeAsync(string name)
        {
            if (options.ReadInclude is null)
            {
                throw new InvalidOperationException("this script can't include other scripts");
            }

            if (includeDepth == MaxIncludeDepth)
            {
                throw new InvalidOperationException(
                    $"scripts include each other more than {MaxIncludeDepth} levels deep: does {name} include itself?");
            }

            var included = Parse(options.ReadInclude(name));
            includeDepth++;

            try
            {
                await interpreter!.RunIncludedAsync(included);
            }
            finally
            {
                includeDepth--;
            }
        }

        interpreter = new ScriptInterpreter(BuiltInFunctions.Create(host, _globals, options.Argument, IncludeAsync));
        await interpreter.RunAsync(statements);
    }

    private static IReadOnlyList<Statement> Parse(string source)
    {
        var tokens = new ScriptLexer().Tokenize(source);

        return new ScriptParser().Parse(tokens);
    }
}
