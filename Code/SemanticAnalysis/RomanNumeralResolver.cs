using AST;

namespace SemanticAnalysis;

public static class RomanNumeralResolver
{
	private static readonly Dictionary<string, int> RomanMap =
		new()
		{
			{ "i", 0 },
			{ "ii", 1 },
			{ "iii", 2 },
			{ "iv", 3 },
			{ "v", 4 },
			{ "vi", 5 },
			{ "vii", 6 }
		};

	public static void Resolve(Song song)
	{
		foreach (var pattern in song.Patterns)
		{
			if (pattern.ScaleName == null)
				continue;

			var scale =
				song.Scales.FirstOrDefault(
					s => s.Name == pattern.ScaleName);

			if (scale == null)
				throw new Exception($"Scale not found: {pattern.ScaleName}");

			foreach (var note in pattern.Notes)
			{
				if (IsRoman(note.Pitch))
				{
					note.Pitch =
						ConvertRoman(note.Pitch, scale);
				}
			}
		}
	}

	private static bool IsRoman(string pitch)
	{
		return pitch.StartsWith("i");
	}

	private static string ConvertRoman(
		string romanPitch,
		Scale scale)
	{
		string numeral =
			new string(
				romanPitch
					.TakeWhile(char.IsLetter)
					.ToArray())
					.ToLower();

		string octave =
			new string(
				romanPitch
					.SkipWhile(char.IsLetter)
					.ToArray());

		int index = RomanMap[numeral];

		return scale.Notes[index] + octave;
	}
}