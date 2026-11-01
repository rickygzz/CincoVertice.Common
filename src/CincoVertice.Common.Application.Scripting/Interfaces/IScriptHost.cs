namespace CincoVertice.Common.Application.Scripting.Interfaces;

/// <summary>
///     What scripts can do on the desktop. Implemented by the UI, so the language can be tested without it.
///     Methods are called on the thread that runs the script.
/// </summary>
public interface IScriptHost
{
    /// <summary>Asks the user for a text.</summary>
    /// <returns>The text entered, or null when the user cancels.</returns>
    Task<string?> AskAsync(string prompt, string defaultText);

    /// <summary>Asks several texts in one dialog, one box per prompt.</summary>
    /// <param name="title">Shown at the top of the dialog.</param>
    /// <param name="prompts">One per box, at least one.</param>
    /// <param name="defaultTexts">The starting text of each box; as many as the prompts.</param>
    /// <returns>The texts entered, one per prompt, or null when the user cancels.</returns>
    Task<IReadOnlyList<string>?> FormAsync(
        string title, IReadOnlyList<string> prompts, IReadOnlyList<string> defaultTexts);

    /// <summary>Shows a message and waits until the user closes it.</summary>
    Task SayAsync(string text);

    /// <summary>Asks a yes/no question.</summary>
    /// <returns>True for yes.</returns>
    Task<bool> ConfirmAsync(string question);

    /// <summary>Opens a web address, file or folder in its default program.</summary>
    void Open(string target);

    /// <summary>Starts a program.</summary>
    void Run(string program, string arguments);

    /// <summary>The text in the clipboard, or empty when it has none.</summary>
    string GetClipboard();

    void SetClipboard(string text);

    /// <summary>Types text into the active window, as if from the keyboard.</summary>
    Task TypeAsync(string text);

    Task WaitAsync(int milliseconds);

    /// <summary>Shows a notification that does not wait for the user.</summary>
    void Notify(string text);
}
