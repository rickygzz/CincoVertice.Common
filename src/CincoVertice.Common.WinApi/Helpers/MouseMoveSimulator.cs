using CincoVertice.Common.WinApi.Libs;
using CincoVertice.Common.WinApi.Libs.Enums;
using CincoVertice.Common.WinApi.Libs.Structs;

namespace CincoVertice.Common.WinApi.Helpers;

/// <summary>
///     Keeps the session active while the user is away: every tick moves the mouse to a random point of the
///     window under the cursor and presses the Print key. When the user moves the mouse, it waits
///     <see cref="MouseMoveSettings.IdleTicksBeforeMove"/> ticks without movement before moving it again.
///     <para>Created stopped. The timer runs on a thread pool thread.</para>
/// </summary>
public sealed class MouseMoveSimulator : IDisposable
{
    private readonly System.Threading.Timer _timer;
    private readonly Random _random = new();

    private MouseMoveSettings _settings = MouseMoveSettings.Default;
    private POINT _prevPosition;
    private int _idleTicksLeft;

    // 1 while a tick is running; a slow move must not overlap the next tick
    private int _ticking;

    public MouseMoveSimulator()
    {
        _timer = new System.Threading.Timer(Timer_Tick, null, Timeout.Infinite, Timeout.Infinite);
    }

    /// <summary>
    ///     The settings, replaced as a whole: a tick reads them once, so even a tick still running after
    ///     <see cref="Stop"/> never mixes old and new values. Interval and MoveImmediately take effect on the next
    ///     <see cref="Start"/>.
    /// </summary>
    /// <exception cref="ArgumentException">
    ///     The settings can't be used. Thrown here, as on the timer's thread an exception would end the application.
    /// </exception>
    public MouseMoveSettings Settings
    {
        get => Volatile.Read(ref _settings);
        set
        {
            if (value.Interval <= 0 || value.IdleTicksBeforeMove < 0 || value.SpeedMin > value.SpeedMax)
            {
                throw new ArgumentException($"These mouse move settings can't be used: {value}", nameof(value));
            }

            Volatile.Write(ref _settings, value);
        }
    }

    public bool IsRunning { get; private set; } = false;

    public void Start()
    {
        MouseMoveSettings settings = Settings;

        _ = User32.GetCursorPos(out _prevPosition);

        _idleTicksLeft = settings.MoveImmediately ? 0 : settings.IdleTicksBeforeMove;

        IsRunning = true;
        _timer.Change(settings.MoveImmediately ? 0 : settings.Interval, settings.Interval);
    }

    public void Stop()
    {
        IsRunning = false;
        _timer.Change(Timeout.Infinite, Timeout.Infinite);
    }

    public void Dispose()
    {
        _timer.Dispose();
    }

    private void Timer_Tick(object? state)
    {
        if (Interlocked.Exchange(ref _ticking, 1) == 1)
        {
            return;
        }

        try
        {
            Tick();
        }
        finally
        {
            Interlocked.Exchange(ref _ticking, 0);
        }
    }

    private void Tick()
    {
        MouseMoveSettings settings = Settings;

        _ = User32.GetCursorPos(out POINT point);

        if (point.X != _prevPosition.X && point.Y != _prevPosition.Y)
        {
            // The user moved the mouse: wait again before moving it
            _prevPosition = point;
            _idleTicksLeft = settings.IdleTicksBeforeMove;

            return;
        }

        if (_idleTicksLeft > 0)
        {
            _idleTicksLeft--;
        }

        if (_idleTicksLeft > 0)
        {
            return;
        }

        nint childUnderCursor = WinUser.WindowFromPoint(point);

        if (User32.GetWindowRect(childUnderCursor, out RECT rec) == 0)
        {
            return;
        }

        int endX = _random.Next(rec.Left, rec.Right);
        int endY = _random.Next(rec.Top, rec.Bottom);
        int speed = _random.Next(settings.SpeedMin, settings.SpeedMax);

        // Remember where the move ends, so it is not taken for the user moving the mouse
        _prevPosition.X = endX;
        _prevPosition.Y = endY;

        InputSimulator.IncrementalMouseMove(point.X, point.Y, endX, endY, speed);

        InputSimulator.SendKey(KeyCode.VK_PRINT);

        _idleTicksLeft = settings.IdleTicksBeforeMove;
    }
}
