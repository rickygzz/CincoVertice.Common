using System.ComponentModel;
using CincoVertice.Common.WinApi.Libs;

namespace CincoVertice.Common.WinApi.Helpers;

/// <summary>
///     Measures how busy the processors and the disks are, from the counters Task Manager shows. Start, wait, then
///     read: the values are for the time in between.
/// </summary>
public sealed class SystemLoadMeter : IDisposable
{
    private const string CpuCounter = @"\Processor(_Total)\% Processor Time";

    // % Disk Time can go far above 100 with several disks or a queue; idle time can't
    private const string DiskIdleCounter = @"\PhysicalDisk(_Total)\% Idle Time";

    private readonly nint _query;
    private readonly nint _cpu;
    private readonly nint _diskIdle;

    /// <exception cref="Win32Exception">The counters can't be read, e.g. when they are turned off.</exception>
    public SystemLoadMeter()
    {
        Check(Pdh.PdhOpenQuery(null, 0, out _query), "open the performance counters");

        try
        {
            Check(Pdh.PdhAddEnglishCounter(_query, CpuCounter, 0, out _cpu), "find the processor counter");
            Check(Pdh.PdhAddEnglishCounter(_query, DiskIdleCounter, 0, out _diskIdle), "find the disk counter");
        }
        catch
        {
            Pdh.PdhCloseQuery(_query);

            throw;
        }
    }

    /// <summary>Starts a measure: what <see cref="Read"/> gives is from here.</summary>
    public void Start()
    {
        Check(Pdh.PdhCollectQueryData(_query), "read the performance counters");
    }

    /// <summary>The average use of the processors, and the share of the time the disks were busy, since Start.</summary>
    /// <returns>Both in percent, 0 to 100.</returns>
    public (double CpuPercent, double DiskPercent) Read()
    {
        Check(Pdh.PdhCollectQueryData(_query), "read the performance counters");

        return (ValueOf(_cpu), 100 - ValueOf(_diskIdle));
    }

    public void Dispose()
    {
        Pdh.PdhCloseQuery(_query);
    }

    private static double ValueOf(nint counter)
    {
        Check(Pdh.PdhGetFormattedCounterValue(counter, Pdh.PdhFmtDouble, out _, out var value), "read a counter");

        return Math.Clamp(value.doubleValue, 0, 100);
    }

    private static void Check(uint status, string what)
    {
        if (status != Pdh.ErrorSuccess)
        {
            throw new Win32Exception(unchecked((int)status), $"Could not {what} (error 0x{status:X8}).");
        }
    }
}
