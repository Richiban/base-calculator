namespace Richiban.BaseCalculator;

public static class Program
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
