using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Checkers;

public class IndentationChecker : ILineChecker
{
    public bool Check(LineModel line)
    {
        int tabs = 0;
        int spaces = 0;
        int indentationLevel = 0;
        bool isIndentationMultipleOfFour = true;

        for (int i = 0; i < line.Content.Length; i++)
        {
            if (!char.IsWhiteSpace(line.Content[i]))
            {
                break;
            }

            if (line.Content[i] == '\t')
            {
                tabs++;

                if (spaces > 0)
                {
                    spaces = 0;
                }

                indentationLevel++;
                isIndentationMultipleOfFour = false;
            }
            else if (line.Content[i] == ' ')
            {
                spaces++;

                if (spaces == 4)
                {
                    spaces = 0;
                    indentationLevel++;
                }
            }
        }

        if (spaces > 0)
        {
            indentationLevel++;
            isIndentationMultipleOfFour = false;
        }

        line.IndentationLevel = indentationLevel;

        if (tabs > 0)
        {
            line.AddError(nameof(Errors.CH0012), Errors.CH0012);

            return true;
        }

        if (!isIndentationMultipleOfFour)
        {
            line.AddError(nameof(Errors.CH0013), Errors.CH0013);

            return true;
        }

        if (line.IndentationLevel != line.PreviousIndentationLevel)
        {
            if (line.IndentationLevel > (line.PreviousIndentationLevel + 1))
            {
                line.AddError(nameof(Errors.CH0014), Errors.CH0014);
            }
        }

        return true;
    }
}
