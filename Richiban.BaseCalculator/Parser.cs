namespace Richiban.BaseCalculator;

public class Parser(int radix)
{
    private readonly Converter _converter = new(radix);

    public Expr Parse(Span<Token> tokens) =>
        tokens switch
        {
            [Token.Literal(var v), Token.Op("+"), _, ..] => new Expr.Add(
                ParseLiteral(v),
                Parse(tokens[2..])),
            [Token.Literal(var v), Token.Op("*"), _, ..] => new Expr.Multiply(
                ParseLiteral(v),
                Parse(tokens[2..])),
            [Token.Literal(var v), Token.Op("-"), _, ..] => new Expr.Subtract(
                ParseLiteral(v),
                Parse(tokens[2..])),
            [Token.Literal(var v), Token.Op("/"), _, ..] => new Expr.Divide(
                ParseLiteral(v),
                Parse(tokens[2..])),
            [_, Token.Op(var op), _, ..] => new Expr.Invalid($"'{op}' is not a valid operator."),
            [Token.Literal(var v)] => ParseLiteral(v),
            [Token.Literal(var l), Token.Literal(var r), ..] => new Expr.Invalid(
                $"Expected an operator between '{l}' and '{r}'"),
            [_, Token.Op] => new Expr.Invalid($"Operators require two values"),
            [Token.Op] => new Expr.Invalid($"Operators require two values"),
            [Token.Op, _] => new Expr.Invalid($"Operators require two values"),
            _ => new Expr.Invalid("Unknown parsing error")
        };

    private Expr ParseLiteral(string input)
    {
        return (_converter.FromBase(input) switch
        {
            { } v => new Expr.Literal(v),
            null => new Expr.Invalid(_converter.InvalidMessage(input))
        });
    }
}
