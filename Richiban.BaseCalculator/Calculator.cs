namespace Richiban.BaseCalculator;

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