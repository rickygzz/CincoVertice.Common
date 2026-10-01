using System.Runtime.InteropServices;

namespace CincoVertice.Common.WinApi.Libs;

public class Kernel32
{
    /// <summary>
    ///     Check if a DLL is present and able to load it
    ///     <para>
    ///         You may want to use System.Runtime.InteropServices.Marshal.PrelinkAll() since it also checks invoke
    ///         methods existence.
    ///     </para>
    /// </summary>
    /// <param name="fileName">
    ///     The name of the module. This can be either a library module (a .dll file) or an executable module (an
    ///     .exe file).
    ///     <para>
    ///         The name specified is the file name of the module and is not related to the name stored in the
    ///         library module itself, as specified by the LIBRARY keyword in the module-definition (.def) file.
    ///     </para>
    ///     <para>If the string specifies a full path, the function searches only that path for the module.</para>
    ///     <para>
    ///         If the string specifies a relative path or a module name without a path, the function uses a
    ///         standard search strategy to find the module; for more information, see the Remarks.
    ///     </para>
    /// </param>
    /// <param name="keeploaded">
    ///     If false, it unloads the library. Otherwise the system unloads the module when its reference count
    ///     reaches zero or when the process terminates (regardless of the reference count).
    /// </param>
    /// <returns>Returns true if the DLL exists and able to load it, otherwise false</returns>
    public static bool IsDllLoaded(string fileName, bool keeploaded = false)
    {
        // [DllImport("some.dll")]
        // private static void SomeMethod();
        // public static void SomeMethodWrapper()
        // {
        //     try
        //     {
        //         SomeMethod();
        //     }
        //     catch (DllNotFoundException)
        //     {
        //         Handle your logic here
        //     }
        // }

        // Increases the reference count and returns a handle
        nint handle = LoadLibrary(fileName);

        if (handle != nint.Zero)
        {
            if (keeploaded == false)
            {
                // Calling the FreeLibrary or FreeLibraryAndExitThread function decrements the reference count.
                FreeLibrary(handle);
            }

            // The system unloads a module when its reference count reaches zero or when the process terminates
            // (regardless of the reference count)
            return true;
        }

        return false;
    }

    // DllImport fields
    // SetLastError Indicates whether the callee calls the SetLastError Win32 API function before returning from the
    // attributed method.
    //           The runtime marshaler calls GetLastError and caches the value returned to prevent it from being
    //           overwritten by other API calls. You can retrieve the error code by calling GetLastWin32Error.
    //           You can retrieve the error code by calling Marshal.GetLastWin32Error
    // ExactSpelling  If false, the entry point name appended with the letter A is invoked when the
    //      DllImportAttribute.CharSet field is set to CharSet.Ansi, and the entry-point name appended with the
    //      letter W is invoked when the DllImportAttribute.CharSet field is set to the CharSet.Unicode
    // EntryPoint You can specify the entry-point name by supplying a string indicating the name of the DLL
    //      containing the entry point, or you can identify the entry point by its ordinal. Ordinals are prefixed
    //      with the # sign, for example, #1. If you omit this field, the common language runtime uses the name of
    //      the .NET method marked with the DllImportAttribute

    /// <summary>
    ///     Loads the specified module into the address space of the calling process. The specified module may cause
    ///     other modules to be loaded.
    ///     <para>
    ///         https://docs.microsoft.com/en-us/windows/desktop/api/libloaderapi/nf-libloaderapi-loadlibrarya
    ///     </para>
    /// </summary>
    /// <param name="lpFileName">
    ///     The name of the module. This can be either a library module (a .dll file) or an executable module (an
    ///     .exe file).
    ///     <para>
    ///         The name specified is the file name of the module and is not related to the name stored in the
    ///         library module itself, as specified by the LIBRARY keyword in the module-definition (.def) file.
    ///     </para>
    ///     <para>
    ///         If the string specifies a full path, the function searches only that path for the module.
    ///     </para>
    ///     <para>
    ///         If the string specifies a relative path or a module name without a path, the function uses a
    ///         standard search strategy to find the module; for more information, see the Remarks.
    ///     </para>
    ///     <para>
    ///         If the function cannot find the module, the function fails. When specifying a path, be sure to use
    ///         backslashes (), not forward slashes (/).
    ///     </para>
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is a handle to the module. If the function fails, the return
    ///     value is NULL. To get extended error information, call GetLastError.
    /// </returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern nint LoadLibrary(string lpFileName);

    /// <summary>
    ///     Frees the loaded dynamic-link library (DLL) module and, if necessary, decrements its reference count.
    ///     When the reference count reaches zero, the module is unloaded from the address space of the calling
    ///     process and the handle is no longer valid.
    /// </summary>
    /// <param name="hLibModule">
    ///     A handle to the loaded library module. The LoadLibrary, LoadLibraryEx, GetModuleHandle, or
    ///     GetModuleHandleEx function returns this handle.
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is nonzero.
    ///     <para>
    ///         If the function fails, the return value is zero.To get extended error information, call the
    ///         GetLastError function.
    ///     </para>
    /// </returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern nint FreeLibrary(nint hLibModule);

    /// <summary>
    /// Retrieves a module handle for the specified module. The module must have been loaded by the calling process.
    /// </summary>
    /// <param name="lpModuleName">
    ///     The name of the loaded module (either a .dll or .exe file). If the file name extension is omitted, the
    ///     default library extension .dll is appended. The file name string can include a trailing point character
    ///     (.) to indicate that the module name has no extension. The string does not have to specify a path. When
    ///     specifying a path, be sure to use backslashes (), not forward slashes (/). The name is compared (case
    ///     independently) to the names of modules currently mapped into the address space of the calling process.
    ///     <para>
    ///         If this parameter is NULL, GetModuleHandle returns a handle to the file used to create the calling
    ///         process (.exe file).
    ///     </para>
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is a handle to the specified module. If the function fails,
    ///     the return value is NULL. To get extended error information, call GetLastError.
    /// </returns>
    [DllImport("kernel32.dll", SetLastError = true)]
    internal static extern nint GetModuleHandle(string lpModuleName);

    /// <summary>
    ///     The memory allocation attributes. If zero is specified, the default is GMEM_FIXED. This parameter can be
    ///     one or more of the following values, except for the incompatible combinations that are specifically
    ///     noted.
    /// </summary>
    [Flags]
    public enum GlobalAllocFlags : uint
    {
        /// <summary>Combines GMEM_MOVEABLE and GMEM_ZEROINIT.</summary>
        GHND = GMEM_MOVEABLE | GMEM_ZEROINIT,

        /// <summary>Allocates fixed memory. The return value is a pointer.</summary>
        GMEM_FIXED = 0x0000,

        /// <summary>
        ///     Allocates movable memory. Memory blocks are never moved in physical memory, but they can be moved
        ///     within the default heap.
        ///     <para>
        ///         The return value is a handle to the memory object. To translate the handle into a pointer, use
        ///         the GlobalLock function.
        ///     </para>
        ///     <para>This value cannot be combined with GMEM_FIXED.</para>
        /// </summary>
        GMEM_MOVEABLE = 0x0002,

        /// <summary>Initializes memory contents to zero.</summary>
        GMEM_ZEROINIT = 0x0040,

        /// <summary>Combines GMEM_FIXED and GMEM_ZEROINIT.</summary>
        GPTR = GMEM_FIXED | GMEM_ZEROINIT
    }

    /// <summary>
    /// Allocates the specified number of bytes from the heap.
    /// </summary>
    /// <param name="flags">
    ///     The memory allocation attributes. If zero is specified, the default is GMEM_FIXED. This parameter can be
    ///     one or more of the following values, except for the incompatible combinations that are specifically
    ///     noted.
    /// </param>
    /// <param name="dwBytes">
    ///     The number of bytes to allocate. If this parameter is zero and the uFlags parameter specifies
    ///     GMEM_MOVEABLE, the function returns a handle to a memory object that is marked as discarded.
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is a handle to the newly allocated memory object.
    /// </returns>
    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    internal static extern nint GlobalAlloc(GlobalAllocFlags flags, uint dwBytes);

    /// <summary>
    /// GlobalFree() Frees the specified global memory object and invalidates its handle.
    /// <para>https://docs.microsoft.com/en-us/windows/desktop/api/winbase/nf-winbase-globalfree</para>
    /// </summary>
    /// <param name="hMem">
    ///     A handle to the global memory object. This handle is returned by either the GlobalAlloc or GlobalReAlloc
    ///     function. It is not safe to free memory allocated with LocalAlloc.
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is NULL.
    ///     <para>
    ///         If the function fails, the return value is equal to a handle to the global memory object. To get
    ///         extended error information, call GetLastError.
    ///     </para>
    /// </returns>
    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    internal static extern nint GlobalFree(nint hMem);

    /// <summary>
    ///     GlobalLock() Locks a global memory object and returns a pointer to the first byte of the object's memory
    ///     block.
    ///     <para>https://docs.microsoft.com/en-us/windows/desktop/api/winbase/nf-winbase-globallock</para>
    /// </summary>
    /// <param name="hMem">
    ///     A handle to the global memory object. This handle is returned by either the GlobalAlloc or GlobalReAlloc
    ///     function
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is a pointer to the first byte of the memory block.
    ///    <para>
    ///     If the function fails, the return value is NULL. To get extended error information, call GetLastError.
    ///     </para>
    /// </returns>
    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    internal static extern nint GlobalLock(nint hMem);

    /// <summary>
    ///     GlobalUnlock() Decrements the lock count associated with a memory object that was allocated with
    ///     GMEM_MOVEABLE. This function has no effect on memory objects allocated with GMEM_FIXED.
    /// </summary>
    /// <param name="hMem">
    ///     A handle to the global memory object. This handle is returned by either the GlobalAlloc or GlobalReAlloc
    ///     function.
    /// </param>
    /// <returns>
    ///     If the memory object is still locked after decrementing the lock count, the return value is a nonzero
    ///     value. If the memory object is unlocked after decrementing the lock count, the function returns zero and
    ///     GetLastError returns NO_ERROR.
    ///     <para>
    ///         If the function fails, the return value is zero and GetLastError returns a value other than NO_ERROR.
    ///     </para>
    /// </returns>
    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    internal static extern bool GlobalUnlock(nint hMem);

    /// <summary>
    ///     Retrieves the current value of the performance counter, which is a high resolution (less than 1us) time
    ///     stamp that can be used for time-interval measurements.
    /// </summary>
    /// <param name="lpPerformanceCount">
    ///     A pointer to a variable that receives the current performance-counter value, in counts.
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is nonzero.
    ///     <para>
    ///         If the function fails, the return value is zero. To get extended error information, call
    ///         GetLastError. On systems that run Windows XP or later, the function will always succeed and will
    ///         thus never return zero.
    ///     </para>
    /// </returns>
    [DllImport("Kernel32.dll", SetLastError = true)]
    public static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

    /// <summary>
    ///     Retrieves the frequency of the performance counter. The frequency of the performance counter is fixed at
    ///     system boot and is consistent across all processors. Therefore, the frequency need only be queried upon
    ///     application initialization, and the result can be cached.
    /// </summary>
    /// <param name="lpFrequency">
    ///     A pointer to a variable that receives the current performance-counter frequency, in counts per second.
    ///     If the installed hardware doesn't support a high-resolution performance counter, this parameter can be
    ///     zero (this will not occur on systems that run Windows XP or later).
    /// </param>
    /// <returns>
    ///     If the installed hardware supports a high-resolution performance counter, the return value is nonzero.
    ///     <para>
    ///         If the function fails, the return value is zero. To get extended error information, call
    ///         GetLastError. On systems that run Windows XP or later, the function will always succeed and will
    ///         thus never return zero.
    ///     </para>
    /// </returns>
    [DllImport("Kernel32.dll")]
    internal static extern bool QueryPerformanceFrequency(out long lpFrequency);
}
