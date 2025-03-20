namespace Richiban.BaseCalculator;

public static class Actions
{
    public static void Main(string[] args)
    {
        var radix = args switch
        {
            ["--radix", var v] => Int32.Parse(v),
            [] => 12,
            _ => throw new Exception("Invalid arguments: " + String.Join(" ", args))
        };

        var repl = new Repl(Console.Out, $"Enter an expression in base {radix}");
        var calculator = new Calculator(radix);

        repl.Run(
            s =>
            {
                var result = calculator.Calculate(s);
                Console.WriteLine(result);
            });
    }
}

public class Calculator(int radix)
{
    private readonly Parser _parser = new(radix);
    private readonly Solver _solver = new(radix);
    private readonly Tokenizer _tokenizer = new();

    public string Calculate(string s)
    {
        var tokens = _tokenizer.Tokenize(s);
        var parsed = _parser.Parse(tokens);
        var result = _solver.Solve(parsed);

        if (result.IsSuccess)
        {
            return $"""
                    Result (base{radix}): {result.RadixResult}
                    Result (base10): {result.DecimalResult}
                    """;
        }

        return $"Error: {result.Error}";
    }
}

public class Tokenizer
{
    public Token[] Tokenize(string input)
    {
        var tokens = input
            .Split(" ")
            .Select(
                token => token switch
                {
                    "+" or "-" or "*" or "/" => (Token)new Token.Op(token),
                    var t => new Token.Other(t)
                })
            .ToArray();

        return tokens;
    }
}

public class Converter(int radix)
{
    private readonly string _digibet = "0123456789ABCDEFGHI";

    public string InvalidMessage =>
        $"Not a valid base {radix} literal. Characters must all be in the range [{_digibet[..radix]}]";

    public string ToBase(int value)
    {
        var result = "";

        while (value > 0)
        {
            result = _digibet[value % radix] + result;
            value /= radix;
        }

        return String.IsNullOrEmpty(result) ? "0" : result;
    }

    public int? FromBase(string input)
    {
        var value = 0;

        foreach (var c in input)
        {
            switch (_digibet.IndexOf(c))
            {
                case -1:
                case var i when i >= radix:
                    return null;

                case var index:
                    value *= radix;
                    value += index;

                    break;
            }
        }

        return value;
    }
}

public class Solver(int radix)
{
    private readonly Converter _converter = new(radix);

    public Result Solve(Expr expression) =>
        expression switch
        {
            Expr.Literal literal => new Result
            {
                DecimalResult = literal.Value,
                RadixResult = _converter.ToBase(literal.Value),
                IsSuccess = true,
            },
            Expr.Add(var left, var right) => Fold(Solve(left), Solve(right), (x, y) => x + y),
            Expr.Subtract(var left, var right) => Fold(Solve(left), Solve(right), (x, y) => x - y),
            Expr.Multiply(var left, var right) => Fold(Solve(left), Solve(right), (x, y) => x * y),
            Expr.Divide(var left, var right) => Fold(Solve(left), Solve(right), (x, y) => x / y),
            Expr.Invalid invalid => new Result { IsSuccess = false, Error = invalid.Message },
            _ => new Result
            {
                IsSuccess = false,
                Error = $"{expression?.GetType().Name} expressions are not supported"
            }
        };

    private Result Fold(Result leftResult, Result rightResult, Func<int, int, int> f)
    {
        if (!leftResult.IsSuccess)
        {
            return leftResult;
        }

        if (!rightResult.IsSuccess)
        {
            return rightResult;
        }

        var decimalResult = f(leftResult.DecimalResult, rightResult.DecimalResult);

        return new Result
        {
            DecimalResult = decimalResult,
            RadixResult = _converter.ToBase(decimalResult),
            IsSuccess = true
        };
    }

    public class Result
    {
        public bool IsSuccess { get; init; }
        public string RadixResult { get; init; }
        public int DecimalResult { get; init; }
        public string? Error { get; init; }
    }
}

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

public abstract record Token
{
    private Token() { }

    public sealed record Op(string Operator) : Token;

    public sealed record Other(string Value) : Token;
}

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

public class Repl(TextWriter @out, string prompt)
{
    public void Run(Action<string> action)
    {
        @out.WriteLine();
        @out.WriteLine(prompt);
        @out.Write("> ");

        if (Console.ReadLine() is { } input)
        {
            action(input);
        }

        Run(action);
    }
}
