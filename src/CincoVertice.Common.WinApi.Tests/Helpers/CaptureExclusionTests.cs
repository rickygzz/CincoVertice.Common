using System.Drawing;
using System.Windows.Forms;
using CincoVertice.Common.WinApi.Helpers;
using Xunit;

namespace CincoVertice.Common.WinApi.Tests.Helpers;

/// <summary>
///     These show a small window on the screen for a moment: a window off the screen would never be in a screenshot,
///     left out or not.
/// </summary>
public class CaptureExclusionTests
{
    private static readonly Color _marker = Color.FromArgb(255, 1, 254, 3);

    private static Form ShowMarker()
    {
        Rectangle area = Screen.PrimaryScreen!.WorkingArea;
        var form = new Form
        {
            BackColor = _marker,
            FormBorderStyle = FormBorderStyle.None,
            ShowInTaskbar = false,
            StartPosition = FormStartPosition.Manual,
            TopMost = true,
            Bounds = new Rectangle(area.Right - 60, area.Bottom - 60, 40, 40),
        };

        form.Show();
        Pump();

        return form;
    }

    private static void Pump()
    {
        for (int i = 0; i < 20; i++)
        {
            Application.DoEvents();
            Thread.Sleep(5);
        }
    }

    /// <summary>How much of the window's color a screenshot of where it is has.</summary>
    private static int MarkerPixels(Form form)
    {
        using Bitmap shot = ScreenCapture.Capture(form.Bounds);
        int count = 0;

        for (int x = 0; x < shot.Width; x++)
        {
            for (int y = 0; y < shot.Height; y++)
            {
                if (shot.GetPixel(x, y).ToArgb() == _marker.ToArgb())
                {
                    count++;
                }
            }
        }

        return count;
    }

    [Fact]
    public void Exclude_LeavesTheWindowOutOfScreenshots_UntilDisposed()
    {
        // Arrange
        using Form form = ShowMarker();
        Assert.SkipWhen(MarkerPixels(form) == 0, "The window can't be seen on the screen, e.g. while it's locked.");

        // Act & Assert: the screenshot taken at once, as a snip takes it
        using (CaptureExclusion? exclusion = CaptureExclusion.Exclude([form.Handle]))
        {
            Assert.NotNull(exclusion);
            Assert.Equal(0, MarkerPixels(form));
            Assert.True(form.Visible);
        }

        Pump();
        Assert.True(MarkerPixels(form) > 0);
    }

    [Fact]
    public void Exclude_WithNoWindows_GivesNothing()
    {
        Assert.Null(CaptureExclusion.Exclude([]));
        Assert.Null(CaptureExclusion.Exclude([0]));
    }

    [Fact]
    public void Dispose_Twice_IsFine()
    {
        // Arrange
        using Form form = ShowMarker();
        CaptureExclusion? exclusion = CaptureExclusion.Exclude([form.Handle]);

        // Act
        exclusion?.Dispose();
        exclusion?.Dispose();

        // Assert: reaching here is the point
        Assert.True(form.Visible);
    }
}
