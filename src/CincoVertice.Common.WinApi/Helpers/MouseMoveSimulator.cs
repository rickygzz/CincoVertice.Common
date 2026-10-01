using CincoVertice.Common.WinApi.Libs.Enums;
using CincoVertice.Common.WinApi.Libs;
using CincoVertice.Common.WinApi.Libs.Structs;

namespace CincoVertice.Common.WinApi.Helpers;

public class MouseMoveSimulator
{
    public int SpeedMin { get; set; } = 20;
    public int SpeedMax { get; set; } = 100;

    public int Interval { get; set; } = 70000;

    private System.Threading.Timer _timer;
    private POINT _prevPosition;

    public MouseMoveSimulator()
    {
        _timer = new(Timer_Tick, null, 0, Interval);
    }

    public void Start()
    {
        _timer.Change(0, Interval);
    }

    public void Stop()
    {
        _timer.Change(Timeout.Infinite, Timeout.Infinite);
    }

    public void Move()
    {
        User32.GetCursorPos(out POINT point);

        if (point.X != _prevPosition.X && point.Y != _prevPosition.Y)
        {
            // Mouse cursor changed position. Do nothing.
            _prevPosition.X = point.X;
            _prevPosition.Y = point.Y;

            return;
        }

        nint childUnderCursor = WinUser.WindowFromPoint(point);

        if (User32.GetWindowRect(childUnderCursor, out RECT rec) == 0)
        {
            return;
        }

        Random rnd = new Random();

        int endX = rnd.Next(rec.Left, rec.Right);
        int endY = rnd.Next(rec.Top, rec.Bottom);
        int speed = rnd.Next(SpeedMin, SpeedMax);

        _prevPosition.X = endX;
        _prevPosition.Y = endY;

        InputSimulator.IncrementalMouseMove(point.X, point.Y, endX, endY, speed);

        InputSimulator.SendKey(KeyCode.VK_PRINT);
    }

    public void Timer_Tick(object? sender)
    {
        Move();
    }
}
