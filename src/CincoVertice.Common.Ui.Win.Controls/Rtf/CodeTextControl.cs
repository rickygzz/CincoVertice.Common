using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using CincoVertice.Common.Ui.Win.Controls.Rtf.Core;
using CincoVertice.Common.Utils.Highlighting;

namespace CincoVertice.Common.Ui.Win.Controls.Rtf;

/// <summary>
/// A RichTextBox for code, with line numbers, syntax colors (SetHighlights) and squiggly lines under the problems given
/// to SetMarks. Numbers and marks are painted over the control, so they don't change the text. Marks and colors stay
/// where they are while the text is edited: give new ones after checking again. WordWrap stays off, since the numbers
/// count display lines. It keeps its own undo (CodeUndoHistory), because RichTextBox's would also undo the colors,
/// and it only takes plain text, so a paste can't bring in other fonts.
/// </summary>
public class CodeTextControl : RichTextBox
{
    private const int WM_SETREDRAW = 0x000B;
    private const int WM_PAINT = 0x000F;
    private const int WM_HSCROLL = 0x0114;
    private const int WM_VSCROLL = 0x0115;
    private const int WM_MOUSEWHEEL = 0x020A;
    private const int EM_SETMARGINS = 0x00D3;
    private const int EC_LEFTMARGIN = 1;
    private const int EM_SETUNDOLIMIT = 0x0452;
    private const int EM_GETSCROLLPOS = 0x04DD;
    private const int EM_SETSCROLLPOS = 0x04DE;

    private const int MinGutterDigits = 2;
    private const int GutterPadding = 6;
    private const int TextGap = 4;
    private const int SquiggleStep = 2;
    private const int SquiggleHeight = 2;
    private const int BlankMarkWidth = 8;

    private readonly ToolTip _toolTip = new();
    private readonly CodeUndoHistory _history = new();

    private IReadOnlyList<CodeMark> _marks = [];
    private IReadOnlyList<CodeSpan> _colored = [];
    private int _coloredLength;
    private int _gutterWidth;
    private int _gutterDigits;
    private bool _hadSelection;
    private string? _shownMessage;

    // While colors are applied, which selects text; and while undo puts back a text, which it has already recorded
    private bool _isColoring;
    private bool _isRestoring;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern nint SendMessage(nint hWnd, int wMsg, nint wParam, nint lParam);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern nint SendMessage(nint hWnd, int wMsg, nint wParam, ref Point lParam);

    public CodeTextControl()
    {
        Multiline = true;
        WordWrap = false;
        ScrollBars = RichTextBoxScrollBars.Both;
        DetectUrls = false;
        HideSelection = false;
        AcceptsTab = true;
        Font = new Font("Consolas", 10);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Color GutterBackColor { get; set; } = Color.FromArgb(240, 240, 240);

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Color GutterForeColor { get; set; } = Color.FromArgb(110, 110, 110);

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public Color SquiggleColor { get; set; } = Color.Red;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IReadOnlyList<CodeMark> Marks => _marks;

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int GutterWidth => _gutterWidth;

    public void SetMarks(IEnumerable<CodeMark> marks)
    {
        _marks = [.. marks];
        Invalidate();
    }

    public void ClearMarks() => SetMarks([]);

    /// <summary>Shows a new text, without colors or marks, that undo can't go back from.</summary>
    public void SetCode(string text)
    {
        _isRestoring = true;

        try
        {
            Text = text;
        }
        finally
        {
            _isRestoring = false;
        }

        _history.Reset(Text);
        _marks = [];
        _colored = [];
        _coloredLength = TextLength;
        Select(0, 0);
        ScrollToCaret();
    }

    /// <summary>Colors the text as given, and everything else with ForeColor.</summary>
    public void SetHighlights(IReadOnlyList<CodeSpan> spans)
    {
        int selectionStart = SelectionStart;
        int selectionLength = SelectionLength;
        var scroll = default(Point);

        _ = SendMessage(Handle, EM_GETSCROLLPOS, 0, ref scroll);
        _ = SendMessage(Handle, WM_SETREDRAW, 0, 0);
        _isColoring = true;

        try
        {
            ApplyColors(spans);
            _colored = spans;
            _coloredLength = TextLength;

            Select(selectionStart, selectionLength);
            _ = SendMessage(Handle, EM_SETSCROLLPOS, 0, ref scroll);
        }
        finally
        {
            _isColoring = false;
            _ = SendMessage(Handle, WM_SETREDRAW, 1, 0);
            Invalidate();
        }
    }

    public void UndoEdit()
    {
        if (_history.Undo() is CodeTextState state)
        {
            Restore(state);
        }
    }

    public void RedoEdit()
    {
        if (_history.Redo() is CodeTextState state)
        {
            Restore(state);
        }
    }

    public void PasteText()
    {
        if (!ReadOnly && Clipboard.ContainsText())
        {
            SelectedText = Clipboard.GetText().Replace("\r\n", "\n");
        }
    }

    public void GoToLine(int line)
    {
        int start = GetFirstCharIndexFromLine(Math.Max(0, line - 1));

        if (start >= 0)
        {
            Select(start, 0);
            ScrollToCaret();
            Focus();
        }
    }

    /// <summary>The message of the marks at a point of the control, or null when there are none.</summary>
    public string? MessageAt(Point point)
    {
        if (_marks.Count == 0 || point.X < _gutterWidth)
        {
            return null;
        }

        int index = GetCharIndexFromPosition(point);
        int line = GetLineFromCharIndex(index);
        string text = Text;
        int column = index - GetFirstCharIndexFromLine(line);
        string lineText = LineText(text, line);

        List<string> messages = [.. _marks
            .Where(mark => mark.Line == line + 1 && Covers(mark.FindRange(lineText), column))
            .Select(mark => mark.Message)];

        return messages.Count == 0 ? null : string.Join(Environment.NewLine, messages);
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        switch (keyData)
        {
            case Keys.Control | Keys.Z:
                UndoEdit();
                return true;

            case Keys.Control | Keys.Y:
            case Keys.Control | Keys.Shift | Keys.Z:
                RedoEdit();
                return true;

            case Keys.Control | Keys.V:
            case Keys.Shift | Keys.Insert:
                PasteText();
                return true;

            // RichTextBox's own shortcuts to align the text, and for line spacing: this text only has colors
            case Keys.Control | Keys.L:
            case Keys.Control | Keys.R:
            case Keys.Control | Keys.E:
            case Keys.Control | Keys.J:
            case Keys.Control | Keys.D1:
            case Keys.Control | Keys.D2:
            case Keys.Control | Keys.D5:
                return true;
        }

        return base.ProcessCmdKey(ref msg, keyData);
    }

    protected override void WndProc(ref Message m)
    {
        base.WndProc(ref m);

        switch (m.Msg)
        {
            case WM_PAINT:
                using (Graphics graphics = Graphics.FromHwnd(Handle))
                {
                    DrawOverlay(graphics);
                }

                break;

            // Scrolling moves the pixels of what was painted over the control, without a full paint
            case WM_HSCROLL:
            case WM_VSCROLL:
            case WM_MOUSEWHEEL:
                Invalidate();
                break;
        }
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        // Each color would be a step of its undo, so there's none: CodeUndoHistory does it
        _ = SendMessage(Handle, EM_SETUNDOLIMIT, 0, 0);

        UpdateGutterWidth(force: true);
    }

    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);

        UpdateGutterWidth(force: true);
    }

    protected override void OnTextChanged(EventArgs e)
    {
        if (_isColoring)
        {
            return;
        }

        if (!_isRestoring)
        {
            _history.Record(Text, SelectionStart);
        }

        base.OnTextChanged(e);

        UpdateGutterWidth(force: false);
        Invalidate();
    }

    protected override void OnSelectionChanged(EventArgs e)
    {
        if (_isColoring)
        {
            return;
        }

        base.OnSelectionChanged(e);

        // Only a selection paints over the marks: repainting for every move of the caret would be wasted
        bool hasSelection = SelectionLength > 0;

        if (hasSelection || _hadSelection)
        {
            Invalidate();
        }

        _hadSelection = hasSelection;
    }

    protected override void OnVScroll(EventArgs e)
    {
        base.OnVScroll(e);

        Invalidate();
    }

    protected override void OnHScroll(EventArgs e)
    {
        base.OnHScroll(e);

        Invalidate();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);

        string? message = MessageAt(e.Location);

        if (message != _shownMessage)
        {
            _shownMessage = message;
            _toolTip.SetToolTip(this, message);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _toolTip.Dispose();
        }

        base.Dispose(disposing);
    }

    protected void DrawOverlay(Graphics graphics)
    {
        int firstLine = GetLineFromCharIndex(GetCharIndexFromPosition(new Point(_gutterWidth, 0)));
        int lastLine = GetLineFromCharIndex(GetCharIndexFromPosition(new Point(_gutterWidth, ClientSize.Height - 1)));

        DrawGutter(graphics, firstLine, lastLine);
        DrawMarks(graphics, firstLine, lastLine);
    }

    private static Color ColorOf(CodeSpanKind kind) => kind switch
    {
        CodeSpanKind.Comment => Color.FromArgb(0, 128, 0),
        CodeSpanKind.Keyword => Color.FromArgb(0, 0, 255),
        CodeSpanKind.Text => Color.FromArgb(163, 21, 21),
        CodeSpanKind.Number => Color.FromArgb(9, 134, 88),
        CodeSpanKind.Function => Color.FromArgb(116, 83, 31),
        CodeSpanKind.Key => Color.FromArgb(0, 16, 128),
        _ => Color.Black,
    };

    /// <summary>
    ///     Colors only between the spans that are the same as last time at the start and, moved by the edit, at the end.
    ///     Coloring takes a while per span, and typing usually changes one or two.
    /// </summary>
    private void ApplyColors(IReadOnlyList<CodeSpan> spans)
    {
        int shift = TextLength - _coloredLength;
        int maxSame = Math.Min(_colored.Count, spans.Count);
        int prefix = 0;

        while (prefix < maxSame && _colored[prefix] == spans[prefix])
        {
            prefix++;
        }

        int suffix = 0;

        while (suffix < maxSame - prefix
            && _colored[_colored.Count - suffix - 1] with { Start = _colored[_colored.Count - suffix - 1].Start + shift }
                == spans[spans.Count - suffix - 1])
        {
            suffix++;
        }

        int start = prefix == 0 ? 0 : spans[prefix - 1].Start + spans[prefix - 1].Length;
        int end = suffix == 0 ? TextLength : spans[spans.Count - suffix].Start;

        Select(start, end - start);
        SelectionColor = ForeColor;

        for (int i = prefix; i < spans.Count - suffix; i++)
        {
            Select(spans[i].Start, spans[i].Length);
            SelectionColor = ColorOf(spans[i].Kind);
        }
    }

    /// <summary>Puts back a text, replacing only what differs, so the colors of the rest stay.</summary>
    private void Restore(CodeTextState state)
    {
        string text = Text;
        int prefix = 0;
        int maxPrefix = Math.Min(text.Length, state.Text.Length);

        while (prefix < maxPrefix && text[prefix] == state.Text[prefix])
        {
            prefix++;
        }

        int suffix = 0;
        int maxSuffix = maxPrefix - prefix;

        while (suffix < maxSuffix && text[^(suffix + 1)] == state.Text[^(suffix + 1)])
        {
            suffix++;
        }

        string replacement = state.Text[prefix..^suffix];

        _isRestoring = true;

        try
        {
            Select(prefix, text.Length - prefix - suffix);
            SelectedText = replacement;
        }
        finally
        {
            _isRestoring = false;
        }

        // Where the change is, so it can be seen
        Select(prefix + replacement.Length, 0);
        ScrollToCaret();
    }

    private void DrawGutter(Graphics graphics, int firstLine, int lastLine)
    {
        using var background = new SolidBrush(GutterBackColor);
        graphics.FillRectangle(background, 0, 0, _gutterWidth, ClientSize.Height);

        const TextFormatFlags flags = TextFormatFlags.Right | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix;

        for (int line = firstLine; line <= lastLine; line++)
        {
            int top = GetPositionFromCharIndex(GetFirstCharIndexFromLine(line)).Y;
            var bounds = new Rectangle(0, top, _gutterWidth - GutterPadding, Font.Height);

            TextRenderer.DrawText(graphics, (line + 1).ToString(), Font, bounds, GutterForeColor, flags);
        }
    }

    private void DrawMarks(Graphics graphics, int firstLine, int lastLine)
    {
        List<CodeMark> visible = [.. _marks.Where(mark => mark.Line - 1 >= firstLine && mark.Line - 1 <= lastLine)];

        if (visible.Count == 0)
        {
            return;
        }

        string text = Text;
        GraphicsState state = graphics.Save();

        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.SetClip(new Rectangle(_gutterWidth, 0, ClientSize.Width - _gutterWidth, ClientSize.Height));

        foreach (CodeMark mark in visible)
        {
            DrawMark(graphics, text, mark);
        }

        graphics.Restore(state);
    }

    private void DrawMark(Graphics graphics, string text, CodeMark mark)
    {
        int line = mark.Line - 1;
        int lineStart = GetFirstCharIndexFromLine(line);
        (int start, int length) = mark.FindRange(LineText(text, line));

        Point from = GetPositionFromCharIndex(lineStart + start);
        int right = length == 0 ? from.X + BlankMarkWidth : RightOf(text, lineStart + start + length - 1);
        int bottom = from.Y + Font.Height;

        using var pen = new Pen(mark.Color ?? SquiggleColor);
        graphics.DrawLines(pen, SquigglePoints(from.X, right, bottom));
    }

    private int RightOf(string text, int index)
    {
        int left = GetPositionFromCharIndex(index).X;

        return left + TextRenderer.MeasureText(text[index].ToString(), Font, Size.Empty, TextFormatFlags.NoPadding).Width;
    }

    private static Point[] SquigglePoints(int left, int right, int bottom)
    {
        List<Point> points = [];

        for (int x = left, i = 0; x < right || points.Count < 2; x += SquiggleStep, i++)
        {
            points.Add(new Point(x, bottom - (i % 2 * SquiggleHeight)));
        }

        return [.. points];
    }

    private string LineText(string text, int line)
    {
        int start = GetFirstCharIndexFromLine(line);
        int end = text.IndexOf('\n', start);

        return end < 0 ? text[start..] : text[start..end];
    }

    private static bool Covers((int Start, int Length) range, int column) =>
        range.Length == 0 || (column >= range.Start && column < range.Start + range.Length);

    private void UpdateGutterWidth(bool force)
    {
        if (!IsHandleCreated)
        {
            return;
        }

        int lineCount = GetLineFromCharIndex(TextLength) + 1;
        int digits = Math.Max(MinGutterDigits, lineCount.ToString().Length);

        if (digits == _gutterDigits && !force)
        {
            return;
        }

        _gutterDigits = digits;
        _gutterWidth = TextRenderer.MeasureText(new string('9', digits), Font).Width + (GutterPadding * 2);

        _ = SendMessage(Handle, EM_SETMARGINS, EC_LEFTMARGIN, _gutterWidth + TextGap);
        Invalidate();
    }
}
