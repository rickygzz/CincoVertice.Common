using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Checkers;

public class DocumentationChecker : ILineChecker
{
    /// <inheritdoc/>
    public bool Check(LineModel line)
    {
        string[] forbiddenDocumentation = ["<summary></summary>", "<returns></returns>", "></param>"];

        if (line.TrimmedContent.StartsWith("///"))
        {
            if (line.TrimmedContent.Length > 3 && line.TrimmedContent[3] != ' ')
            {
                line.AddError(nameof(Errors.CH0020), Errors.CH0020);

                return false;
            }

            foreach (var token in forbiddenDocumentation)
            {
                if (line.TrimmedContent.Contains(token))
                {
                    line.AddError(nameof(Errors.CH0030), Errors.CH0030);

                    return false;
                }
            }
        }

        return true;
    }
}
