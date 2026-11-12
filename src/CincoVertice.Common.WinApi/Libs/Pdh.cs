using System.Runtime.InteropServices;

namespace CincoVertice.Common.WinApi.Libs;

/// <summary>
///     Performance Data Helper: reads the performance counters Windows keeps, such as the use of the processors.
///     Every function returns <see cref="ErrorSuccess"/> or an error code.
/// </summary>
public static partial class Pdh
{
    public const uint ErrorSuccess = 0;

    /// <summary>Asks <see cref="PdhGetFormattedCounterValue"/> for the value as a double.</summary>
    public const uint PdhFmtDouble = 0x00000200;

    /// <summary>Creates a query of counters on this computer.</summary>
    /// <param name="szDataSource">Null for the live counters, not a log file.</param>
    /// <param name="dwUserData">A value to identify the query; 0 when not needed.</param>
    /// <param name="phQuery">The query; close it with <see cref="PdhCloseQuery"/>.</param>
    [LibraryImport("pdh.dll", EntryPoint = "PdhOpenQueryW", StringMarshalling = StringMarshalling.Utf16)]
    public static partial uint PdhOpenQuery(string? szDataSource, nint dwUserData, out nint phQuery);

    /// <summary>
    ///     Adds a counter to the query by its English path, e.g. \Processor(_Total)\% Processor Time, which works
    ///     whatever the language of Windows.
    /// </summary>
    [LibraryImport("pdh.dll", EntryPoint = "PdhAddEnglishCounterW", StringMarshalling = StringMarshalling.Utf16)]
    public static partial uint PdhAddEnglishCounter(
        nint hQuery,
        string szFullCounterPath,
        nint dwUserData,
        out nint phCounter);

    /// <summary>
    ///     Takes a sample of every counter in the query. A rate, such as a percent of time, needs two samples: it is
    ///     the value between the last two.
    /// </summary>
    [LibraryImport("pdh.dll")]
    public static partial uint PdhCollectQueryData(nint hQuery);

    /// <summary>The value of a counter between the last two samples of its query.</summary>
    /// <param name="hCounter">The counter.</param>
    /// <param name="dwFormat">How to give the value, e.g. <see cref="PdhFmtDouble"/>.</param>
    /// <param name="lpdwType">The type of the counter; may be ignored.</param>
    /// <param name="pValue">The value.</param>
    [LibraryImport("pdh.dll")]
    public static partial uint PdhGetFormattedCounterValue(
        nint hCounter,
        uint dwFormat,
        out uint lpdwType,
        out PDH_FMT_COUNTERVALUE pValue);

    /// <summary>Closes a query and its counters.</summary>
    [LibraryImport("pdh.dll")]
    public static partial uint PdhCloseQuery(nint hQuery);

    /// <summary>
    ///     A counter's value as asked with <see cref="PdhFmtDouble"/>; the other members of the native union are
    ///     not used here.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PDH_FMT_COUNTERVALUE
    {
        /// <summary>0 or 1 when the value is valid.</summary>
        public uint CStatus;

        public double doubleValue;
    }
}
