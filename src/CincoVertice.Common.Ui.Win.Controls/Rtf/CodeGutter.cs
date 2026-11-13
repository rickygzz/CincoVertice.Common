using System.Runtime.InteropServices;

namespace CincoVertice.Common.Ui.Win.Controls.Rtf;

/// <summary>
/// The line numbers of a CodeTextControl, in a window of their own over its left margin. Painted over the text box
/// itself, they flickered on every scroll: the text box moves the pixels of what's painted on it, then repaints the
/// margin blank before they're painted again. Inside the text box, it would be moved too, as the text box scrolls the
/// windows in it. So it's beside it, on the same parent and over it: the text box can neither move it nor paint over
/// it, and it paints double-buffered, all at once. What the mouse does over it goes to the text box, as if it weren't
/// there.
/// </summary>
internal sealed class CodeGutter : Control
{
    private const int WM_MOUSEFIRST = 0x0200;
    private const int WM_MOUSELAST = 0x020E;

    private readonly CodeTextControl _owner;

    public CodeGutter(CodeTextControl owner)
    {
        _owner = owner;
        TabStop = false;
        SetStyle(
            ControlStyles.UserPaint
                | ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw,
            true);
        SetStyle(ControlStyles.Selectable, false);
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern nint SendMessage(nint hWnd, int wMsg, nint wParam, nint lParam);

    protected override void OnPaint(PaintEventArgs e)
    {
        _owner.PaintGutter(e.Graphics);
    }

    protected override void WndProc(ref Message m)
    {
        // At the same place, since it sits at the text box's corner: a click there selects lines, the wheel scrolls
        if (m.Msg is >= WM_MOUSEFIRST and <= WM_MOUSELAST && _owner.IsHandleCreated)
        {
            m.Result = SendMessage(_owner.Handle, m.Msg, m.WParam, m.LParam);

            return;
        }

        base.WndProc(ref m);
    }
}
