namespace Richiban.BaseCalculator;

public abstract record Expr
{
    private Expr() { }

    public sealed record Literal(int Value) : Expr;

    public sealed record Add(Expr Left, Expr Right) : Expr;

    public sealed record Multiply(Expr Left, Expr Right) : Expr;

    public sealed record Subtract(Expr Left, Expr Right) : Expr;

    public sealed record Divide(Expr Left, Expr Right) : Expr;

    public sealed record Invalid(string Message) : Expr;
}