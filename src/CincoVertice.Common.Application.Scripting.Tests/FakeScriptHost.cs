using CincoVertice.Common.Application.Scripting.Interfaces;
using CincoVertice.Common.Application.Scripting.Models;

namespace CincoVertice.Common.Application.Scripting.Tests;

/// <summary>
///     Records what a script asks the desktop to do. ask() and confirm() take their answers from queues;
///     a null answer to ask() means the user cancelled.
/// </summary>
internal sealed class FakeScriptHost : IScriptHost
{
    public Queue<string?> Answers { get; } = new();

    public Queue<bool> Confirmations { get; } = new();

    public string Clipboard { get; set; } = string.Empty;

    /// <summary>Each call as text, e.g. "say: Hello" or "open: https://example.com".</summary>
    public List<string> Calls { get; } = [];

    public Task<string?> AskAsync(string prompt, string defaultText)
    {
        Calls.Add($"ask: {prompt} [{defaultText}]");

        return Task.FromResult(Answers.Dequeue());
    }

    /// <summary>The answers form() gets; null means the user cancelled.</summary>
    public Queue<IReadOnlyList<string>?> FormAnswers { get; } = new();

    // A list field shows as Prompt={a, b}, or Prompt={a, b} b with an item to select; a text field as Prompt=default
    public Task<IReadOnlyList<string>?> FormAsync(string title, IReadOnlyList<FormFieldModel> fields)
    {
        var shown = fields.Select(f => f.Choices is null
            ? $"{f.Prompt}={f.DefaultText}"
            : $"{f.Prompt}={{{string.Join(", ", f.Choices)}}}{(f.DefaultText.Length > 0 ? " " + f.DefaultText : "")}");
        Calls.Add($"form: {title} [{string.Join(" | ", shown)}]");

        return Task.FromResult(FormAnswers.Dequeue());
    }

    public Task SayAsync(string text)
    {
        Calls.Add($"say: {text}");

        return Task.CompletedTask;
    }

    public Task<bool> ConfirmAsync(string question)
    {
        Calls.Add($"confirm: {question}");

        return Task.FromResult(Confirmations.Dequeue());
    }

    /// <summary>When set, Open() throws it, like a file that does not exist.</summary>
    public Exception? OpenFailure { get; set; }

    public void Open(string target)
    {
        if (OpenFailure is not null)
        {
            throw OpenFailure;
        }

        Calls.Add($"open: {target}");
    }

    public void Run(string program, string arguments) => Calls.Add($"run: {program} [{arguments}]");

    public string GetClipboard() => Clipboard;

    public void SetClipboard(string text) => Calls.Add($"copy: {text}");

    public Task TypeAsync(string text)
    {
        Calls.Add($"type: {text}");

        return Task.CompletedTask;
    }

    public Task PasteTextAsync(string text)
    {
        Calls.Add($"paste: {text}");

        return Task.CompletedTask;
    }

    public Task PressAsync(string keys, int times)
    {
        if (keys == "Foo")
        {
            throw new ArgumentException("'Foo' is not a key.");
        }

        Calls.Add($"press: {keys} [{times}]");

        return Task.CompletedTask;
    }

    public string ActiveWindowTitle { get; set; } = string.Empty;

    public string GetActiveWindowTitle() => ActiveWindowTitle;

    /// <summary>How many zones there are; a higher one throws, as the app does.</summary>
    public int ZoneCount { get; set; } = 3;

    public void MoveActiveWindowToZone(int zone)
    {
        if (zone > ZoneCount)
        {
            throw new ArgumentException($"there's no zone {zone}: there are {ZoneCount}.");
        }

        Calls.Add($"tozone: {zone}");
    }

    public Task WaitAsync(int milliseconds)
    {
        Calls.Add($"wait: {milliseconds}");

        return Task.CompletedTask;
    }

    /// <summary>What each measure of the load gives, in turn; once empty, the computer is quiet (0% and 0%).</summary>
    public Queue<SystemLoadModel> Loads { get; } = new();

    /// <summary>How many times the load was measured, and so how long system.waitidle waited.</summary>
    public int LoadMeasures { get; private set; }

    public Task<SystemLoadModel> MeasureLoadAsync(int milliseconds)
    {
        LoadMeasures++;

        return Task.FromResult(Loads.Count > 0 ? Loads.Dequeue() : new SystemLoadModel(0, 0));
    }

    public void Notify(string text) => Calls.Add($"notify: {text}");
}
