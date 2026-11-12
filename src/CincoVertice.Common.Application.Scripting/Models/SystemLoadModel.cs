namespace CincoVertice.Common.Application.Scripting.Models;

/// <summary>
///     How busy the computer was over a while.
/// </summary>
/// <param name="CpuPercent">The average use of all the processors, 0 to 100.</param>
/// <param name="DiskPercent">The share of the time the disks were busy, 0 to 100.</param>
public sealed record SystemLoadModel(decimal CpuPercent, decimal DiskPercent);
