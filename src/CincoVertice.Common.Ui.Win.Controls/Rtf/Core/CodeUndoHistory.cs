namespace CincoVertice.Common.Ui.Win.Controls.Rtf.Core;

/// <summary>
///     The undo and redo of <see cref="CodeTextControl"/>: the texts it had, with where the caret was. Typing a word or
///     deleting character by character is one step, as in other editors.
/// </summary>
public sealed class CodeUndoHistory
{
    private const int MaxStates = 500;

    private readonly List<CodeTextState> _states = [new CodeTextState(string.Empty, 0)];

    private int _current;
    private EditRun _run;

    private enum EditRun
    {
        None,
        Typing,
        Deleting,
    }

    public void Reset(string text)
    {
        _states.Clear();
        _states.Add(new CodeTextState(text, 0));
        _current = 0;
        _run = EditRun.None;
    }

    public void Record(string text, int caret)
    {
        CodeTextState previous = _states[_current];

        if (text == previous.Text)
        {
            return;
        }

        EditRun run = RunOf(previous, text, caret);

        _states.RemoveRange(_current + 1, _states.Count - _current - 1);

        if (run != EditRun.None && run == _run && Continues(run, previous.Caret, caret))
        {
            _states[_current] = new CodeTextState(text, caret);

            return;
        }

        _run = run;
        _states.Add(new CodeTextState(text, caret));
        _current++;

        if (_states.Count > MaxStates)
        {
            _states.RemoveAt(0);
            _current--;
        }
    }

    public CodeTextState? Undo()
    {
        if (_current == 0)
        {
            return null;
        }

        _run = EditRun.None;

        return _states[--_current];
    }

    public CodeTextState? Redo()
    {
        if (_current == _states.Count - 1)
        {
            return null;
        }

        _run = EditRun.None;

        return _states[++_current];
    }

    private static EditRun RunOf(CodeTextState previous, string text, int caret)
    {
        string before = previous.Text;

        if (text.Length == before.Length + 1
            && caret > 0
            && caret <= text.Length
            && !char.IsWhiteSpace(text[caret - 1])
            && IsSameAround(before, text, caret - 1, caret))
        {
            return EditRun.Typing;
        }

        if (text.Length == before.Length - 1
            && caret <= text.Length
            && IsSameAround(text, before, caret, caret + 1))
        {
            return EditRun.Deleting;
        }

        return EditRun.None;
    }

    /// <summary>Whether this edit is right where the last one ended, and not after moving the caret.</summary>
    private static bool Continues(EditRun run, int previousCaret, int caret) => run == EditRun.Typing
        ? previousCaret == caret - 1
        : previousCaret == caret || previousCaret == caret + 1;

    /// <summary>Whether longer is shorter with one character added at the given position.</summary>
    private static bool IsSameAround(string shorter, string longer, int shorterEnd, int longerStart) =>
        string.CompareOrdinal(shorter, 0, longer, 0, shorterEnd) == 0
        && string.CompareOrdinal(shorter, shorterEnd, longer, longerStart, longer.Length) == 0;
}
