using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Interfaces;

public interface ICodeCheckerService
{
    void CheckFile(string filePath, out List<ErrorModel> errors);
}
