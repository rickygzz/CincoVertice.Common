using System.Runtime.InteropServices;

namespace CincoVertice.Common.WinApi.Libs;

public class GDIplus
{
    /// <summary>
    ///     The GdiplusStartup function initializes Windows GDI+. Call GdiplusStartup before making any other GDI+
    ///     calls, and call GdiplusShutdown when you have finished using GDI+.
    /// </summary>
    /// <param name="token">
    ///     Pointer to a ULONG_PTR that receives a token. Pass the token to GdiplusShutdown when you have finished
    ///     using GDI+.
    /// </param>
    /// <param name="input">
    ///     Pointer to a GdiplusStartupInput structure that contains the GDI+ version, a pointer to a debug callback
    ///     function, a Boolean value that specifies whether to suppress the background thread, and a Boolean value
    ///     that specifies whether to suppress external image codecs.
    /// </param>
    /// <param name="output">
    ///     Pointer to a GdiplusStartupOutput structure that receives a pointer to a notification hook function and
    ///     a pointer to a notification unhook function. If the SuppressBackgroundThread data member of the input
    ///     parameter is FALSE, then this parameter can be NULL.
    /// </param>
    /// <returns>
    ///     If the function succeeds, it returns Ok (0), which is an element of the Status enumeration. If the
    ///     function fails, it returns one of the other elements of the Status enumeration.
    /// </returns>
    [DllImport("gdiplus.dll", ExactSpelling = true)]
    internal static extern int GdiplusStartup(out nint token, ref GdipStartupInput input, out GdipStartupOutput output);

    /// <summary>
    ///     The GdiplusShutdown function cleans up resources used by Windows GDI+. Each call to GdiplusStartup
    ///     should be paired with a call to GdiplusShutdown.
    /// </summary>
    /// <param name="token">Token returned by a previous call to GdiplusStartup.</param>
    [DllImport("gdiplus.dll", ExactSpelling = true)]
    internal static extern void GdiplusShutdown(nint token);

    /// <summary>
    /// Creates a Bitmap object based on a BITMAPINFO structure and an array of pixel data.
    /// </summary>
    /// <param name="gdiBitmapInfo">Pointer to a GDI BITMAPINFO</param>
    /// <param name="gdiBitmapData">Pointer to an array of bytes that contains the pixel data</param>
    /// <param name="pBitmap">Pointer to a Bitmap object</param>
    /// <returns>
    ///     If the function succeeds, it returns Ok (0), which is an element of the Status enumeration. If thw
    ///     function fails, it returns one of the other elements of the Status enumeration.
    /// </returns>
    [DllImport("gdiplus.dll", ExactSpelling = true)]
    internal static extern int GdipCreateBitmapFromGdiDib(nint gdiBitmapInfo, nint gdiBitmapData, out nint pBitmap);

    /// <summary>
    /// Saves this image to a file.
    /// </summary>
    /// <param name="pBmp">Pointer to the Image object.</param>
    /// <param name="file">
    ///     Pointer to a null-terminated unicode string that specifies the path name for the saved image.
    /// </param>
    /// <param name="clsidEncoder">Pointer to a CLSID that specifies the encoder to use to save the image.</param>
    /// <param name="pEncoderParams">
    ///     Pointer to an EncoderParameters structure that holds parameters used by the encoder. The default value
    ///     is NULL.
    /// </param>
    /// <returns>
    ///     If the function succeeds, it returns Ok (0), which is an element of the Status enumeration. If the
    ///     function fails, it returns one of the other elements of the Status enumeration.
    /// </returns>
    [DllImport("gdiplus.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
    internal static extern int GdipSaveImageToFile(nint pBmp, string file, [In] ref Guid clsidEncoder, EncoderParameters pEncoderParams);

    /// <summary>
    /// Saves this image to a file.
    /// </summary>
    /// <param name="pBmp">Pointer to the Image object.</param>
    /// <param name="file">
    ///     Pointer to a null-terminated unicode string that specifies the path name for the saved image.
    /// </param>
    /// <param name="clsidEncoder">Pointer to a CLSID that specifies the encoder to use to save the image.</param>
    /// <param name="pEncoderParams">
    ///     Pointer to an EncoderParameters structure that holds parameters used by the encoder. The default value
    ///     is NULL.
    /// </param>
    /// <returns>
    ///     If the function succeeds, it returns Ok (0), which is an element of the Status enumeration. If the
    ///     function fails, it returns one of the other elements of the Status enumeration.
    /// </returns>
    [DllImport("gdiplus.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
    internal static extern int GdipSaveImageToFile(nint pBmp, string file, [In] ref Guid clsidEncoder, nint pEncoderParams);

    /// <summary>
    /// Disposes the specified Image object.
    /// </summary>
    /// <param name="pBmp">Pointer to the Image object to dispose.</param>
    /// <returns>
    ///     If the function succeeds, it returns Ok (0), which is an element of the Status enumeration. If the
    ///     function fails, it returns one of the other elements of the Status enumeration.
    /// </returns>
    [DllImport("gdiplus.dll", ExactSpelling = true)]
    internal static extern int GdipDisposeImage(nint pBmp);

    /// <summary>
    ///     The GdiplusStartupInput structure holds a block of arguments that are required by the GdiplusStartup
    ///     function.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct GdipStartupInput
    {
        /// <summary>Specifies the version of GDI+. Must be 1.</summary>
        public int GdiplusVersion;

        /// <summary>
        ///     Pointer to a callback function that GDI+ can call, on debug builds, for assertions and warnings.
        ///     The default value is NULL.
        /// </summary>
        public nint DebugEventCallback;

        /// <summary>
        ///     Boolean value that specifies whether to suppress the GDI+ background thread. If you set this member
        ///     to TRUE, GdiplusStartup returns (in its output parameter) a pointer to a hook function and a pointer
        ///     to an unhook function. You must call those functions appropriately to replace the background thread.
        ///     If you do not want to be responsible for calling the hook and unhook functions, set this member to
        ///     FALSE. The default value is FALSE.
        /// </summary>
        public bool SuppressBackgroundThread;

        /// <summary>
        ///     Boolean value that specifies whether you want GDI+ to suppress external image codecs. GDI+ version
        ///     1.0 does not support external image codecs, so this parameter is ignored.
        /// </summary>
        public bool SuppressExternalCodecs;
    }

    /// <summary>
    ///     The GdiplusStartup function uses the GdiplusStartupOutput structure to return (in its output parameter)
    ///     a pointer to a hook function and a pointer to an unhook function. If you set the
    ///     SuppressBackgroundThread member of the input parameter to TRUE, then you are responsible for calling
    ///     those functions to replace the Windows GDI+ background thread.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct GdipStartupOutput
    {
        /// <summary>Receives a pointer to a hook function.</summary>
        public nint NotificationHook;

        /// <summary>Receives a pointer to an unhook function.</summary>
        public nint NotificationUnhook;
    }

    /// <summary>
    ///     An EncoderParameter object holds a parameter that can be passed to an image encoder. An EncoderParameter
    ///     object can also be used to receive a list of possible values supported by a particular parameter of a
    ///     particular image encoder.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct EncoderParameter
    {
        /// <summary>
        ///     Identifies the parameter category. GUIDs that represent various parameter categories
        ///     (EncoderCompression, EncoderColorDepth, and the like) are defined in Gdiplusimaging.h
        /// </summary>
        public Guid GUID;

        /// <summary>Number of values in the array pointed to by the Value data member.</summary>
        public uint NumberOfValues;

        /// <summary>
        ///     Identifies the data type of the parameter. The EncoderParameterValueType enumeration in
        ///     Gdiplusenums.h defines several possible value types.
        /// </summary>
        public uint Type;

        /// <summary>
        ///     Pointer to an array of values. Each value has the type specified by the Type data member.
        /// </summary>
        public nint Value;
    }

    /// <summary>
    ///     An EncoderParameters object is an array of EncoderParameter objects along with a data member that
    ///     specifies the number of EncoderParameter objects in the array.
    ///     When you create an EncoderParameters object, you must allocate enough memory to hold all of the
    ///     EncoderParameter objects that will eventually be placed in the array. For example, if you want to create
    ///     an EncoderParameters object that will hold an array of five EncoderParameter objects, you should use
    ///     code similar to the following:
    ///     EncoderParameters* pEncoderParameters =
    ///         (EncoderParameters*) malloc(sizeof(EncoderParameters) + 4 * sizeof(EncoderParameter));
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 2)]
    internal struct EncoderParameters
    {
        /// <summary>Number of EncoderParameter objects in the array</summary>
        public uint Count;

        /// <summary>Array of EncoderParameter objects.</summary>
        public EncoderParameter[] Parameter;
    }
}
