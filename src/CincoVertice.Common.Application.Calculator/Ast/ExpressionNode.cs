namespace CincoVertice.Common.Application.Calculator.Ast;

/// <summary>
///     Node of the expression syntax tree built by <see cref="ExpressionParser"/>.
///     <para>ToString() prints the node in prefix notation, e.g. 1+2*3 is (+ 1 (* 2 3)).</para>
/// </summary>
/// <param name="Position">Zero-based position in the expression, used to report evaluation errors.</param>
public abstract record ExpressionNode(int Position);
