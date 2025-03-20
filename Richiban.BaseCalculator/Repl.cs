namespace Richiban.BaseCalculator;

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