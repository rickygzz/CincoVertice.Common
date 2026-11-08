namespace CincoVertice.Common.WinApi.Libs.Enums;

/// <summary>
///     Ternary raster oprations. Describes how three boolean values should combine to form an output boolean.
///     <para>https://wiki.winehq.org/Ternary_Raster_Ops</para>
/// </summary>
public enum RasterOperationCode : uint
{
    /// <summary>
    ///     Fills the destination rectangle using the color associated with index 0 in the physical palette. (This color
    ///     is black for the default physical palette.)
    ///     <para>dest = BLACK</para>
    /// </summary>
    BLACKNESS = 0x00000042,

    /// <summary>
    ///     Includes any windows that are layered on top of your window in the resulting image. By default, the image
    ///     only contains your window. Note that this generally cannot be used for printing device contexts.
    /// </summary>
    CAPTUREBLT = 0x40000000,

    /// <summary>
    ///     Inverts the destination rectangle.
    ///     <para>dest = (NOT dest)</para>
    /// </summary>
    DSTINVERT = 0x00550009,

    /// <summary>
    ///     Merges the colors of the source rectangle with the brush currently selected in hdcDest, by using the
    ///     Boolean AND operator.
    ///     <para>dest = (source AND pattern)</para>
    /// </summary>
    MERGECOPY = 0x00C000CA,

    /// <summary>
    ///     Merges the colors of the inverted source rectangle with the colors of the destination rectangle by using the
    ///     Boolean OR operator.
    ///     <para>dest = (NOT source) OR dest</para>
    /// </summary>
    MERGEPAINT = 0x00BB0226,

    /// <summary>Prevents the bitmap from being mirrored.</summary>
    NOMIRRORBITMAP = 0x80000000,

    /// <summary>Copies the inverted source rectangle to the destination.
    /// <para>dest = (NOT source)</para></summary>
    NOTSRCCOPY = 0x00330008,

    /// <summary>
    ///     Combines the colors of the source and destination rectangles by using the Boolean OR operator and then
    ///     inverts the resultant color.
    ///     <para>dest = (NOT src) AND (NOT dest)</para>
    /// </summary>
    NOTSRCERASE = 0x001100A6,

    /// <summary>Copies the brush currently selected in hdcDest, into the destination bitmap.
    /// <para>dest = pattern</para></summary>
    PATCOPY = 0x00F00021,

    /// <summary>
    ///     Combines the colors of the brush currently selected in hdcDest, with the colors of the destination rectangle
    ///     by using the Boolean XOR operator.
    ///     <para>dest = pattern XOR dest</para>
    /// </summary>
    PATINVERT = 0x005A0049,

    /// <summary>
    ///     Combines the colors of the brush currently selected in hdcDest, with the colors of the inverted source
    ///     rectangle by using the Boolean OR operator. The result of this operation is combined with the colors of the
    ///     destination rectangle by using the Boolean OR operator.
    ///     <para>dest = DPSnoo</para>
    /// </summary>
    PATPAINT = 0x00FB0A09,

    /// <summary>
    ///     Combines the colors of the source and destination rectangles by using the Boolean AND operator.
    ///     <para>dest = source AND dest</para>
    /// </summary>
    SRCAND = 0x008800C6,

    /// <summary>
    ///     Copies the source rectangle directly to the destination rectangle.
    ///     <para>dest = source</para>
    /// </summary>
    SRCCOPY = 0x00CC0020,

    /// <summary>
    ///     Combines the inverted colors of the destination rectangle with the colors of the source rectangle by using
    ///     the Boolean AND operator.
    ///     <para>dest = source AND (NOT dest)</para>
    /// </summary>
    SRCERASE = 0x00440328,

    /// <summary>
    ///     Combines the colors of the source and destination rectangles by using the Boolean XOR operator.
    ///     <para>dest = source XOR dest</para>
    /// </summary>
    SRCINVERT = 0x00660046,

    /// <summary>
    ///     Combines the colors of the source and destination rectangles by using the Boolean OR operator.
    ///     <para>dest = source OR dest</para>
    /// </summary>
    SRCPAINT = 0x00EE0086,

    /// <summary>
    ///     Fills the destination rectangle using the color associated with index 1 in the physical palette. (This color
    ///     is white for the default physical palette.)
    ///     <para>dest = WHITE</para>
    /// </summary>
    WHITENESS = 0x00FF0062
}
