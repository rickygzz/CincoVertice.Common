namespace CincoVertice.Common.Application.Calculator.Models;

public record Token(TokenType Type, string Value, int Position)
{
    public override string ToString() => $"{Type}('{Value}') at {Position+1}";
}
