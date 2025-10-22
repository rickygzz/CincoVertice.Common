namespace CincoVertice.Common.WinApi.Helpers;

public static class ProcessHelper
{
    public static bool IsProcessCurrentlyRunning()
    {
        string assemblyLocation = System.Reflection.Assembly.GetEntryAssembly()?.Location ?? string.Empty;
        string file = Path.GetFileNameWithoutExtension(assemblyLocation);
        
        System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(file);
        System.Diagnostics.Process thisProcess = System.Diagnostics.Process.GetCurrentProcess();

        foreach (System.Diagnostics.Process process in processes)
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
