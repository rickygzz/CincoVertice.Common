using CincoVertice.Common.Application.Scripting.Models;

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

    /// <summary>Asks several things in one dialog: a text box, or a fixed list to pick from, per field.</summary>
    /// <param name="title">Shown at the top of the dialog.</param>
    /// <param name="fields">One per box, at least one.</param>
    /// <returns>The text entered or the item picked, one per field, or null when the user cancels.</returns>
    Task<IReadOnlyList<string>?> FormAsync(string title, IReadOnlyList<FormFieldModel> fields);

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

    /// <summary>
    ///     Pastes text into the active window with Ctrl+V, as plain text, then puts back what the clipboard held.
    /// </summary>
    Task PasteTextAsync(string text);

    /// <summary>Presses keys in the active window, such as "Ctrl+V" or "Enter".</summary>
    /// <param name="keys">The keys, written as in hotkeys.cfg; a key without modifiers is allowed.</param>
    /// <param name="times">How many times to press them, at least 1.</param>
    /// <exception cref="ArgumentException">The keys can't be read; the message says why.</exception>
    Task PressAsync(string keys, int times);

    /// <summary>The title of the window the user works in; empty when there is none or it has no title.</summary>
    string GetActiveWindowTitle();

    /// <summary>Moves the window the user works in to one of the window zones of its monitor.</summary>
    /// <param name="zone">Which, from 1.</param>
    /// <exception cref="ArgumentException">
    ///     There's no such zone, or no window to move; the message says why.
    /// </exception>
    void MoveActiveWindowToZone(int zone);

    Task WaitAsync(int milliseconds);

    /// <summary>Measures how busy the computer is over the next milliseconds, which it waits.</summary>
    Task<SystemLoadModel> MeasureLoadAsync(int milliseconds);

    /// <summary>Shows a notification that does not wait for the user.</summary>
    void Notify(string text);
}
