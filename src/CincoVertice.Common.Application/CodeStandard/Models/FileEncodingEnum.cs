namespace CincoVertice.Common.Application.CodeStandard.Models;

/// <summary>How a fix writes a file's text.</summary>
public enum FileEncodingEnum
{
    /// <summary>As the file was: UTF-8 with or without a BOM, or the encoding of its BOM.</summary>
    Keep = 0,

    /// <summary>UTF-8 without a byte-order mark.</summary>
    Utf8 = 1,

    /// <summary>UTF-8 with a byte-order mark (EF BB BF), as Visual Studio saves files.</summary>
    Utf8Bom = 2,
}
