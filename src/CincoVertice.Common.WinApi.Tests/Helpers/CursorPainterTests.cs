using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using CincoVertice.Common.WinApi.Helpers;
using CincoVertice.Common.WinApi.Libs;
using CincoVertice.Common.WinApi.Libs.Structs;
using Xunit;

namespace CincoVertice.Common.WinApi.Tests.Helpers;

/// <summary>With the real mouse pointer, which it moves for a moment and puts back.</summary>
public class CursorPainterTests
{
    private static int NotWhite(Bitmap image, Rectangle area)
    {
        int count = 0;

        for (int x = area.Left; x < area.Right; x++)
        {
            for (int y = area.Top; y < area.Bottom; y++)
            {
                if (image.GetPixel(x, y).ToArgb() != Color.White.ToArgb())
                {
                    count++;
                }
            }
        }

        return count;
    }

    [Fact]
    public void Draw_PutsThePointerWhereItIs_RelativeToTheImage()
    {
        // Arrange
        User32.GetCursorPos(out POINT before);
        Rectangle screen = Screen.PrimaryScreen!.Bounds;
        var point = new Point(screen.Left + 300, screen.Top + 300);
        User32.SetCursorPos(point.X, point.Y);

        using var image = new Bitmap(200, 200);
        using var graphics = Graphics.FromImage(image);
        graphics.Clear(Color.White);

        try
        {
            // Act: the image starts 100 pixels up and left of the pointer
            bool drawn = CursorPainter.Draw(graphics, new Point(point.X - 100, point.Y - 100));

            // Assert: the pointer is at the middle of the image, not at its corner
            // Windows hides the pointer while the user types in some programs, which this would then be
            var info = new User32.CURSORINFO { cbSize = Marshal.SizeOf<User32.CURSORINFO>() };
            User32.GetCursorInfo(ref info);
            string state = info.flags == 0 ? "hidden" : "shown";
            Assert.True(drawn, $"The pointer wasn't drawn; Windows says it's {state}.");
            Assert.True(NotWhite(image, new Rectangle(95, 95, 40, 40)) > 20);
            Assert.Equal(0, NotWhite(image, new Rectangle(0, 0, 60, 60)));
        }
        finally
        {
            User32.SetCursorPos(before.X, before.Y);
        }
    }
}
