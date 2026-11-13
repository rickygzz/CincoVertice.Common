using System.Drawing;
using System.Windows.Forms;
using CincoVertice.Common.WinApi.Helpers;
using Xunit;

namespace CincoVertice.Common.WinApi.Tests.Helpers;

/// <summary>With a real window, shown for a moment, and the real monitors.</summary>
public class WindowPlacementTests
{
    private static Form ShowWindow()
    {
        Rectangle area = Screen.PrimaryScreen!.WorkingArea;
        var form = new Form
        {
            ShowInTaskbar = false,
            StartPosition = FormStartPosition.Manual,
            Bounds = new Rectangle(area.Left + 50, area.Top + 50, 300, 200),
        };

        form.Show();
        Application.DoEvents();

        return form;
    }

    [Fact]
    public void MoveVisibleBounds_MakesWhatShowsOfTheWindowFillTheBounds()
    {
        // Arrange
        using Form form = ShowWindow();
        Rectangle area = Screen.PrimaryScreen!.WorkingArea;
        var bounds = new Rectangle(area.Left + 100, area.Top + 80, 400, 250);

        // Act
        WindowHelper.MoveVisibleBounds(form.Handle, bounds);
        Application.DoEvents();

        // Assert: the invisible borders are outside the bounds
        Assert.Equal(bounds, WindowHelper.GetVisibleBounds(form.Handle));
        Assert.True(form.Bounds.Contains(bounds));
    }

    [Fact]
    public void MoveVisibleBounds_OfAMaximizedWindow_RestoresIt()
    {
        // Arrange
        using Form form = ShowWindow();
        form.WindowState = FormWindowState.Maximized;
        Application.DoEvents();
        Rectangle area = Screen.PrimaryScreen!.WorkingArea;
        var bounds = new Rectangle(area.Left + 20, area.Top + 20, 350, 220);

        // Act
        WindowHelper.MoveVisibleBounds(form.Handle, bounds);
        Application.DoEvents();

        // Assert
        Assert.Equal(FormWindowState.Normal, form.WindowState);
        Assert.Equal(bounds, WindowHelper.GetVisibleBounds(form.Handle));
    }

    [Fact]
    public void IsMovableWindow_OfAWindowWithATitleBar_IsTrue_OfAToolWindowOrNone_False()
    {
        // Arrange
        using Form form = ShowWindow();
        using var tool = new Form { FormBorderStyle = FormBorderStyle.SizableToolWindow, ShowInTaskbar = false };
        tool.Show();

        // Act and assert
        Assert.True(WindowHelper.IsMovableWindow(form.Handle));
        Assert.False(WindowHelper.IsMovableWindow(tool.Handle));
        Assert.False(WindowHelper.IsMovableWindow(0));
    }

    [Fact]
    public void GetWorkAreas_HasEveryMonitorsWorkArea()
    {
        // Act
        IReadOnlyList<Rectangle> areas = MonitorHelper.GetWorkAreas();

        // Assert
        Assert.Equal(
            Screen.AllScreens.Select(screen => screen.WorkingArea).OrderBy(a => a.X).ThenBy(a => a.Y),
            areas.OrderBy(a => a.X).ThenBy(a => a.Y));
    }

    [Fact]
    public void GetMonitors_HasEveryMonitorsBounds_WithItsWorkArea()
    {
        // Act
        IReadOnlyList<(Rectangle Bounds, Rectangle WorkArea)> monitors = MonitorHelper.GetMonitors();

        // Assert
        Assert.Equal(
            Screen.AllScreens.Select(screen => (screen.Bounds, screen.WorkingArea)).OrderBy(m => m.Bounds.X),
            monitors.OrderBy(m => m.Bounds.X));
    }

    [Fact]
    public void GetWorkArea_IsTheOneOfTheWindowsMonitor()
    {
        // Arrange
        using Form form = ShowWindow();

        // Act
        Rectangle area = MonitorHelper.GetWorkArea(form.Handle);

        // Assert
        Assert.Equal(Screen.FromHandle(form.Handle).WorkingArea, area);
    }
}
