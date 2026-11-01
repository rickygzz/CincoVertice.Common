using System.Globalization;
using CincoVertice.Common.Application.Scripting.Interfaces;

namespace CincoVertice.Common.Application.Scripting.Functions;

/// <summary>
///     The functions available in scripts, grouped in modules (module.function); only the conversions number() and
///     text() are global, since they take any value. Names are case-insensitive.
/// </summary>
public static class BuiltInFunctions
{
    // A longer wait is more likely a mistake (seconds instead of milliseconds) than intended
    private const int MaxWaitMilliseconds = 10 * 60 * 1000;

    // More key presses than this is more likely a mistake than intended
    private const int MaxPressTimes = 100;

    /// <param name="host">What scripts do on the desktop.</param>
    /// <param name="globals">The global variables.</param>
    /// <param name="argument">What script.argument() returns; empty for none.</param>
    /// <param name="includeAsync">Runs the script with the given name with the caller's variables.</param>
    public static IReadOnlyDictionary<string, ScriptFunction> Create(
        IScriptHost host,
        IScriptVariableStore globals,
        string argument,
        Func<string, Task> includeAsync)
    {
        return new ScriptFunction[]
        {
            // dialog: ask the user, wait for the answer
            new("dialog.ask", 1, 2, async a => await host.AskAsync(a.Text(0), a.Text(1, string.Empty))
                ?? throw new ScriptStopException()),
            // form(title, prompts, defaults): one dialog with a box per prompt; returns the answers as a list
            new("dialog.form", 2, 3, a => FormAsync(host, a)),
            new("dialog.message", 1, 1, async a => { await host.SayAsync(a.Text(0)); return ScriptValue.Nothing; }),
            new("dialog.confirm", 1, 1, async a => await host.ConfirmAsync(a.Text(0))),

            new("clipboard.gettext", 0, 0, _ => Task.FromResult<object>(host.GetClipboard())),
            new("clipboard.settext", 1, 1, a => Done(() => host.SetClipboard(a.Text(0)))),
            new("clipboard.clear", 0, 0, _ => Done(() => host.SetClipboard(string.Empty))),
            new("clipboard.pasteastext", 0, 0, _ => PasteAsync(host, host.GetClipboard())),
            // paste(text): faster than keyboard.type for long texts, and editors don't auto-indent it
            new("clipboard.paste", 1, 1, a => PasteAsync(host, a.Text(0))),

            new("keyboard.type", 1, 1, async a => { await host.TypeAsync(a.Text(0)); return ScriptValue.Nothing; }),
            // press("Ctrl+V"), press("Tab", 3): keys as in hotkeys.cfg
            new("keyboard.press", 1, 2, async a =>
            {
                await host.PressAsync(a.Text(0), ToTimes(a));

                return ScriptValue.Nothing;
            }),

            new("window.title", 0, 0, _ => Task.FromResult<object>(host.GetActiveWindowTitle())),

            new("shell.open", 1, 1, a => Done(() => host.Open(a.Text(0)))),
            new("shell.run", 1, 2, a => Done(() => host.Run(a.Text(0), a.Text(1, string.Empty)))),

            new("tray.notify", 1, 1, a => Done(() => host.Notify(a.Text(0)))),

            new("script.wait", 1, 1, async a =>
            {
                await host.WaitAsync(ToMilliseconds(a));

                return ScriptValue.Nothing;
            }),
            new("script.stop", 0, 0, _ => throw new ScriptStopException()),
            // argument(): the value after the shortcut in hotkeys.cfg; argument(default) when there is none
            new("script.argument", 0, 1, a => Value(argument.Length > 0 ? argument : a.Text(0, string.Empty))),
            new("script.include", 1, 1, a => IncludeAsync(includeAsync, a)),

            // global: values kept between runs and shared by all scripts
            new("global.set", 2, 2, a => Done(() => globals.Set(GlobalName(a), a.Value(1)))),
            new("global.get", 1, 2, a => Value(GetGlobal(globals, a))),
            new("global.has", 1, 1, a => Value(globals.TryGet(GlobalName(a), out _))),
            new("global.remove", 1, 1, a => Done(() => globals.Remove(GlobalName(a)))),
            new("global.names", 0, 0, _ => Value(globals.GetNames().Cast<object>().ToList().AsReadOnly())),

            new("text.length", 1, 1, a => Value(NotList(a, "list.length").Length)),
            new("text.upper", 1, 1, a => Value(a.Text(0).ToUpperInvariant())),
            new("text.lower", 1, 1, a => Value(a.Text(0).ToLowerInvariant())),
            new("text.trim", 1, 1, a => Value(a.Text(0).Trim())),
            new("text.replace", 3, 3, a => Value(Replace(a))),
            // Ignores case: text.contains("Hello", "hello") is true
            new("text.contains", 2, 2, a => Value(
                NotList(a, "list.contains").Contains(a.Text(1), StringComparison.OrdinalIgnoreCase))),
            // For web addresses: text.urlencode("a b&c") is "a%20b%26c"
            new("text.urlencode", 1, 1, a => Value(Uri.EscapeDataString(a.Text(0)))),

            new("list.length", 1, 1, a => Value(a.List(0).Count)),
            // An item equal to the value (== rules)
            new("list.contains", 2, 2, a => Value(a.List(0).Any(item => ScriptValue.AreEqual(item, a.Value(1))))),

            // Global conversions
            new("text", 1, 1, a => Value(a.Text(0))),
            new("number", 1, 1, a => Value(ToNumber(a))),
        }
        .ToDictionary(f => f.Name, StringComparer.OrdinalIgnoreCase);
    }

    private static Task<object> Value(object value) => Task.FromResult(value);

    private static Task<object> Value(int value) => Task.FromResult<object>((decimal)value);

    private static Task<object> Done(Action action)
    {
        action();

        return Task.FromResult(ScriptValue.Nothing);
    }

    /// <summary>
    ///     Pastes the text as plain text; the clipboard keeps what it had. Nothing happens for an empty text, e.g.
    ///     pasteastext() when the clipboard has no text.
    /// </summary>
    private static async Task<object> PasteAsync(IScriptHost host, string text)
    {
        if (text.Length > 0)
        {
            await host.PasteTextAsync(text);
        }

        return ScriptValue.Nothing;
    }

    private static async Task<object> FormAsync(IScriptHost host, ScriptArguments a)
    {
        List<string> prompts = [.. a.List(1).Select(ScriptValue.ToText)];
        List<string> defaults = a.Count > 2 ? [.. a.List(2).Select(ScriptValue.ToText)] : [];

        if (prompts.Count == 0)
        {
            throw a.Error("the list of prompts is empty");
        }

        if (defaults.Count > prompts.Count)
        {
            throw a.Error($"there are {defaults.Count} default texts for {prompts.Count} prompts");
        }

        // Prompts without a default start empty
        defaults.AddRange(Enumerable.Repeat(string.Empty, prompts.Count - defaults.Count));

        var answers = await host.FormAsync(a.Text(0), prompts, defaults) ?? throw new ScriptStopException();

        return answers.Cast<object>().ToList().AsReadOnly();
    }

    /// <summary>
    ///     Argument 1 as text, but not a list, which would otherwise be measured or searched as "[a, b]".
    /// </summary>
    private static string NotList(ScriptArguments a, string listFunction)
    {
        if (a.Value(0) is IReadOnlyList<object>)
        {
            throw a.Error($"argument 1 is a list: use {listFunction}()");
        }

        return a.Text(0);
    }

    private static string Replace(ScriptArguments a)
    {
        if (a.Text(1).Length == 0)
        {
            throw a.Error("the text to replace can't be empty");
        }

        return a.Text(0).Replace(a.Text(1), a.Text(2), StringComparison.Ordinal);
    }

    private static int ToMilliseconds(ScriptArguments a)
    {
        decimal milliseconds = a.Number(0);

        if (milliseconds < 0 || milliseconds > MaxWaitMilliseconds)
        {
            throw a.Error($"the time must be from 0 to {MaxWaitMilliseconds} milliseconds");
        }

        return (int)milliseconds;
    }

    /// <summary>
    ///     Runs another script. An error in it is reported at this call, with its place in the other script.
    /// </summary>
    private static async Task<object> IncludeAsync(Func<string, Task> includeAsync, ScriptArguments a)
    {
        string name = a.Text(0).Trim();

        try
        {
            await includeAsync(name);
        }
        catch (ScriptException ex)
        {
            throw a.Error($"in {name}, line {ex.Line}, column {ex.Column}: {ex.Message}");
        }

        return ScriptValue.Nothing;
    }

    /// <summary>
    ///     Argument 1 as the name of a global variable: a text that is not empty.
    /// </summary>
    private static string GlobalName(ScriptArguments a)
    {
        if (a.Value(0) is not string name)
        {
            throw a.Error($"the name must be a text, not {ScriptValue.Describe(a.Value(0))}");
        }

        if (name.Trim().Length == 0)
        {
            throw a.Error("the name can't be empty");
        }

        return name.Trim();
    }

    /// <summary>
    ///     The value of a global variable, or the default (argument 2) when it was never set.
    /// </summary>
    private static object GetGlobal(IScriptVariableStore globals, ScriptArguments a)
    {
        string name = GlobalName(a);

        if (globals.TryGet(name, out object? value))
        {
            return value;
        }

        return a.Count > 1
            ? a.Value(1)
            : throw a.Error($"there is no global variable '{name}'; global.get(\"{name}\", value) gives a value then");
    }

    private static int ToTimes(ScriptArguments a)
    {
        if (a.Count < 2)
        {
            return 1;
        }

        decimal times = a.Number(1);

        if (times < 1 || times > MaxPressTimes || times != decimal.Truncate(times))
        {
            throw a.Error($"the times must be a whole number from 1 to {MaxPressTimes}, not {times}");
        }

        return (int)times;
    }

    /// <summary>
    ///     Converts a text such as "12.5" to a number; a number is returned as it is.
    /// </summary>
    private static decimal ToNumber(ScriptArguments a)
    {
        string text = a.Text(0).Trim();

        if (!decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal number))
        {
            throw a.Error($"'{text}' is not a number");
        }

        return number;
    }
}
