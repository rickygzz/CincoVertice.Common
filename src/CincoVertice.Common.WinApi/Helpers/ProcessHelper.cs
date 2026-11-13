using System.Diagnostics;

namespace CincoVertice.Common.WinApi.Helpers;

public static class ProcessHelper
{
    public static bool IsProcessCurrentlyRunning()
    {
        // The process name, not the entry assembly's location: that is empty in a single-file build
        Process thisProcess = Process.GetCurrentProcess();
        Process[] processes = Process.GetProcessesByName(thisProcess.ProcessName);

        foreach (Process process in processes)
        {
            if (process.Id != thisProcess.Id)
            {
                WindowHelper.ActivateWindow(process.MainWindowHandle);

                return true;
            }
        }

        return false;
    }
}
