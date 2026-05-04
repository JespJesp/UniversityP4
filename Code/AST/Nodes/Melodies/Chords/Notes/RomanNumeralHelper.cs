namespace Ast.Nodes.Melodies.Chords;

public static class RomanNumeralHelper
{
    private static readonly Dictionary<string, int> Map = new()
    {
        { "I", 1 }, { "II", 2 }, { "III", 3 },
        { "IV", 4 }, { "V", 5 }, { "VI", 6 }, { "VII", 7 }
    };

    public static bool IsRomanNumeral(string value)
    {
        return Map.ContainsKey(value.ToUpper());
    }

    public static int ToDegree(string numeral)
    {
        return Map[numeral.ToUpper()];
    }
}