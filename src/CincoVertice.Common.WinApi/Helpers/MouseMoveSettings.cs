namespace CincoVertice.Common.WinApi.Helpers;

/// <summary>
///     How <see cref="MouseMoveSimulator"/> moves the mouse. Immutable, so the simulator's timer thread always sees one
///     whole set of settings.
/// </summary>
/// <param name="Interval">Milliseconds between ticks.</param>
/// <param name="IdleTicksBeforeMove">Ticks without the user moving the mouse before it is moved.</param>
/// <param name="SpeedMin">
///     The fastest move, at most <paramref name="SpeedMax"/>; higher is slower. See
///     <see cref="InputSimulator.IncrementalMouseMove"/>.
/// </param>
/// <param name="SpeedMax">The slowest move.</param>
/// <param name="MoveImmediately">
///     True to move the mouse as soon as the simulator starts; false to first wait
///     <paramref name="IdleTicksBeforeMove"/> ticks, as if the user had just moved it.
/// </param>
public sealed record MouseMoveSettings(
    int Interval,
    int IdleTicksBeforeMove,
    int SpeedMin,
    int SpeedMax,
    bool MoveImmediately)
{
    public static MouseMoveSettings Default { get; } = new(5000, 20, 40, 100, true);
}
