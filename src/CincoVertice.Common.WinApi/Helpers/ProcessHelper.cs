using System.Diagnostics;

namespace CincoVertice.Common.WinApi.Helpers;

public static class ProcessHelper
{
    public static bool IsProcessCurrentlyRunning()
    {
        string assemblyLocation = System.Reflection.Assembly.GetEntryAssembly()?.Location ?? string.Empty;
        string file = Path.GetFileNameWithoutExtension(assemblyLocation);

        Process[] processes = Process.GetProcessesByName(file);
        Process thisProcess = Process.GetCurrentProcess();

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
