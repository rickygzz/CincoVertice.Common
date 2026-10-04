using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Helper;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Checkers;

/// <summary>
///     Reports invisible characters and Unicode spaces, with their column, since they cannot be seen in the editor.
/// </summary>
public class InvisibleCharacterChecker : ILineChecker
{
    public bool Check(LineModel line)
    {
        for (int i = 0; i < line.Content.Length; i++)
        {
            char c = line.Content[i];

            if (UnicodeCharacters.IsInvisible(c))
            {
                AddError(line, nameof(Errors.CH0025), Errors.CH0025, c, i);
            }
            else if (UnicodeCharacters.IsUnicodeSpace(c))
            {
                AddError(line, nameof(Errors.CH0026), Errors.CH0026, c, i);
            }
        }

        // Never stops parsing: the rest of the line still gets checked
        return true;
    }

    private static void AddError(LineModel line, string code, string message, char c, int index)
    {
        line.AddError(
            code,
            message
                .Replace("{character}", UnicodeCharacters.Describe(c))
                .Replace("{column}", (index + 1).ToString()));
    }
}
