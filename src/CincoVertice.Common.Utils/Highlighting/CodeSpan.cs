namespace CincoVertice.Common.Utils.Highlighting;

/// <summary>A piece of code to color. Start and Length are positions in the text; it may span lines.</summary>
public readonly record struct CodeSpan(int Start, int Length, CodeSpanKind Kind);
