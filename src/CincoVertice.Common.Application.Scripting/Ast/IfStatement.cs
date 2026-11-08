namespace CincoVertice.Common.Application.Scripting.Ast;

/// <summary>
///     if ... elseif ... else ... end: runs the body of the first branch whose condition is true, or else Else.
/// </summary>
public record IfStatement(IReadOnlyList<IfBranch> Branches, IReadOnlyList<Statement> Else, int Line, int Column)
    : Statement(Line, Column);
