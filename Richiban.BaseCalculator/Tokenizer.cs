namespace Richiban.BaseCalculator;

public class Tokenizer
{
    public Token[] Tokenize(string input)
    {
        var tokens = input
            .Split(" ", StringSplitOptions.RemoveEmptyEntries)
            .Select(
                token => token switch
                {
                    [var c] when !char.IsLetterOrDigit(c) => (Token)new Token.Op(c.ToString()),
                    var t => new Token.Literal(t)
                })
            .ToArray();

        return tokens;
    }
}
