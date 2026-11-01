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

    public static IReadOnlyDictionary<string, ScriptFunction> Create(IScriptHost host)
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

            new("keyboard.type", 1, 1, async a => { await host.TypeAsync(a.Text(0)); return ScriptValue.Nothing; }),

            new("shell.open", 1, 1, a => Done(() => host.Open(a.Text(0)))),
            new("shell.run", 1, 2, a => Done(() => host.Run(a.Text(0), a.Text(1, string.Empty)))),

            new("tray.notify", 1, 1, a => Done(() => host.Notify(a.Text(0)))),

            new("script.wait", 1, 1, async a =>
            {
                await host.WaitAsync(ToMilliseconds(a));

                return ScriptValue.Nothing;
            }),
            new("script.stop", 0, 0, _ => throw new ScriptStopException()),

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
