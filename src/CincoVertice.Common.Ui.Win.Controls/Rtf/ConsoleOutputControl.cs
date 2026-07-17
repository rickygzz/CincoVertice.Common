using CincoVertice.Common.Ui.Rtf.Models;
using CincoVertice.Common.Ui.Win.Controls.Rtf.Core;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace CincoVertice.Common.Ui.Win.Controls.Rtf;

[SupportedOSPlatform("windows10.0")]
public class ConsoleOutputControl : RichTextBox
{
    private readonly RtfText _rtf;
    private readonly ContextMenuStrip _contextMenu;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int SendMessage(nint hWnd, int wMsg, nint wParam, nint lParam);
    private const int WM_VSCROLL = 0x115;
    private const int SB_BOTTOM = 7;

    public ConsoleOutputControl()
    {
        Multiline = true;
        DetectUrls = false;

        _rtf = new RtfText(Font.Name, Font.Size);

        CreateControl();

        _contextMenu = new ContextMenuStrip();
        InitializeContextMenu();
    }
    protected override void OnFontChanged(EventArgs e)
    {
        _rtf.UpdateDefaultFont(Font.Name, Font.Size);
    }

    public void AddText(string text, RtfFormat? formatModel = null, bool updateContent = false)
    {
        _rtf.AddText(text, formatModel);

        if (updateContent)
        {
            UpdateText(true);
        }
    }

    public void AddText(string text, bool updateContent = false)
    {
        _rtf.AddText(text);

        if (updateContent)
        {
            UpdateText(true);
        }
    }

    public void AddNewLine(bool updateContent = false)
    {
        _rtf.AddNewLine();

        if (updateContent)
        {
            UpdateText(true);
        }
    }

    public void AddColor(params Color[] colors)
    {
        _rtf.AddColor(colors);
    }

    public void AddFont(params Font[] fonts)
    {
        _rtf.AddFont(fonts);
    }

    public void UpdateText(bool scrolltoBottom = false)
    {
        Rtf = _rtf.Rtf(updateContent: true);

        if (scrolltoBottom)
        {
            ScrollToBottom();
        }
    }

    public void ScrollToBottom()
    {
        _ = SendMessage(Handle, WM_VSCROLL, SB_BOTTOM, nint.Zero);
    }

    private void InitializeContextMenu()
    {
        var clearMenuItem = new ToolStripMenuItem("Clear Text");
        clearMenuItem.Click += ClearMenuItem_Click;
        _contextMenu.Items.Add(clearMenuItem);

        ContextMenuStrip = _contextMenu;
    }

    private void ClearMenuItem_Click(object? sender, EventArgs e)
    {
        Clear();
    }
}
