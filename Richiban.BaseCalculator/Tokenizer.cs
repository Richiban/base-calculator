namespace Richiban.BaseCalculator;

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