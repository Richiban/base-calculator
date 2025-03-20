namespace Richiban.BaseCalculator;

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