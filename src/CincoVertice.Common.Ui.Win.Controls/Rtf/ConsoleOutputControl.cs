using System.Runtime.InteropServices;
using CincoVertice.Common.Ui.Rtf.Models;
using CincoVertice.Common.Ui.Rtf.Themes;
using CincoVertice.Common.Ui.Win.Controls.Rtf.Core;

namespace CincoVertice.Common.Ui.Win.Controls.Rtf;

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
        HideSelection = false;

        _rtf = new RtfText(Font.Name, Font.Size);

        CreateControl();

        _contextMenu = new ContextMenuStrip();
        InitializeContextMenu();
    }

    protected override void OnFontChanged(EventArgs e)
    {
        _rtf.UpdateDefaultFont(Font.Name, Font.Size);
    }

    public void AddText(
        string text,
        RtfFormat? formatModel = null,
        bool updateContent = false)
    {
        _rtf.AddText(text, formatModel);

        if (updateContent)
        {
            UpdateText(scrollToBottom: true);
        }
    }

    public void AddNewLine(bool updateContent = false)
    {
        _rtf.AddNewLine();

        if (updateContent)
        {
            UpdateText(scrollToBottom: true);
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

    public void UpdateText(bool scrollToBottom = false)
    {
        Rtf = _rtf.Rtf(updateContent: true);

        if (scrollToBottom)
        {
            ScrollToBottom();
        }
    }

    public void SetTheme(IRtfTheme theme)
    {
        BackColor = theme.Colors["Background"].ToColor();

        _rtf.UpdateTheme(theme);
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

    public void SetText(
        string text,
        RtfFormat? formatModel = null,
        bool updateContent = false)
    {
        _rtf.ClearContent();
        _rtf.AddText(text, formatModel);

        if (updateContent)
        {
            UpdateText(scrollToBottom: true);
        }
    }

    public void ClearContent(bool updateContent = true)
    {
        _rtf.ClearContent();
        Rtf = _rtf.Rtf(updateContent);
    }

    private void ClearMenuItem_Click(object? sender, EventArgs e)
    {
        ClearContent();
    }
}
