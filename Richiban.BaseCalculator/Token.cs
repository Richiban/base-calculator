namespace Richiban.BaseCalculator;

public abstract record Token
{
    private Token() { }

    public sealed record Op(string Operator) : Token;

    public sealed record Literal(string Value) : Token;
}