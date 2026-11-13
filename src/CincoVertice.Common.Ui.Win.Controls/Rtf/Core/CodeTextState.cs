namespace CincoVertice.Common.Ui.Win.Controls.Rtf.Core;

/// <summary>A text <see cref="CodeUndoHistory"/> can go back to, with where the caret was.</summary>
public readonly record struct CodeTextState(string Text, int Caret);
