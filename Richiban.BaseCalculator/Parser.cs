namespace Richiban.BaseCalculator;

public class Parser(int radix)
{
    private readonly Converter _converter = new(radix);

    public Expr Parse(Span<Token> tokens)
    {
        switch (tokens)
        {
            case [Token.Other(var v), Token.Op("+"), _, ..]:
                return new Expr.Add(ParseLiteral(v), Parse(tokens[2..]));

            case [Token.Other(var v), Token.Op("*"), _, ..]:
                return new Expr.Multiply(ParseLiteral(v), Parse(tokens[2..]));

            case [Token.Other(var v), Token.Op("-"), _, ..]:
                return new Expr.Subtract(ParseLiteral(v), Parse(tokens[2..]));

            case [Token.Other(var v), Token.Op("/"), _, ..]:
                return new Expr.Divide(ParseLiteral(v), Parse(tokens[2..]));

            case [Token.Other(var v)]:
                return ParseLiteral(v);

            case [Token.Other(var l), Token.Other(var r), ..]:
                return new Expr.Invalid($"Expected an operator between '{l}' and '{r}'");

            case [Token.Other(var l), Token.Op(var op)]:
                return new Expr.Invalid($"The operator {op} requires two values'");

            default:
                return new Expr.Invalid("Unknown parsing error");
        }
    }

    private Expr ParseLiteral(string input)
    {
        return (_converter.FromBase(input) switch
        {
            { } v => new Expr.Literal(v),
            null => new Expr.Invalid(_converter.InvalidMessage)
        });
    }
}