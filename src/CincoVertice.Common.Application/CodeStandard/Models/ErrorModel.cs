namespace CincoVertice.Common.Application.CodeStandard.Models;

public class ErrorModel
{
    public int Line { get; set; }

    public string Code { get; set; } = string.Empty;

    public string Message { get; set; } = string.Empty;
}
