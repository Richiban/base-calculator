namespace Richiban.BaseCalculator;

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