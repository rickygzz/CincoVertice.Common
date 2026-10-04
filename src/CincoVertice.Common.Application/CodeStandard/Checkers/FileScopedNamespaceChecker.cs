using CincoVertice.Common.Application.CodeStandard.Constants;
using CincoVertice.Common.Application.CodeStandard.Interfaces;
using CincoVertice.Common.Application.CodeStandard.Models;

namespace CincoVertice.Common.Application.CodeStandard.Checkers;

/// <summary>
///     CH0029: namespaces must be file-scoped (namespace Name;), not blocks (namespace Name { }).
///     Fixed by FileScopedNamespaceFixer.
/// </summary>
public class FileScopedNamespaceChecker : ILineChecker
{
    private const string Keyword = "namespace ";

    /// <inheritdoc/>
    public bool Check(LineModel line)
    {
        // TrimmedContent no longer has a trailing comment, so "namespace Name; // comment" ends with ;
        if (line.TrimmedContent.StartsWith(Keyword, StringComparison.Ordinal) && !line.TrimmedContent.EndsWith(';'))
        {
            line.AddError(nameof(Errors.CH0029), Errors.CH0029);
        }

        return true;
    }
}
