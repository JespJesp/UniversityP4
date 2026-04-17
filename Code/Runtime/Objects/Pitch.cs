using System.Text.RegularExpressions;

namespace Runtime.Objects;

public class Pitch : RuntimeObject
{
	/// <summary>
	/// Goes from C=0 to B=11.
	/// </summary>
	public int PitchClass = 0;
	public int Octave = 5;

	/// <summary>
	/// Converts the input pitch string to lower case before processing it. For example, this pitch string could be "C5" or "c#3".
	/// Throws an error if the input string cannot be converted to a pitch.
	/// </summary>
	public static Pitch FromString(string pitchString)
	{
		Pitch pitch = new();

		string toLower = pitchString.ToLower();
		string[] parts = Regex.Split(toLower, @"(\d+)"); // Splits the string when it reaches a digit
		string pitchClass = parts[0];
		string octave = parts[1];

		switch (pitchClass)
		{
			case "c": pitch.PitchClass = 0; break;
			case "c#" or "db": pitch.PitchClass = 1; break;
			case "d": pitch.PitchClass = 2; break;
			case "d#" or "eb": pitch.PitchClass = 3; break;
			case "e": pitch.PitchClass = 4; break;
			case "f": pitch.PitchClass = 5; break;
			case "f#" or "gb": pitch.PitchClass = 6; break;
			case "g": pitch.PitchClass = 7; break;
			case "g#" or "ab": pitch.PitchClass = 8; break;
			case "a": pitch.PitchClass = 9; break;
			case "a#" or "bb": pitch.PitchClass = 10; break;
			case "b": pitch.PitchClass = 11; break;
			default: throw new Exception($"Pitch class '{pitchClass}' is not recognized.");
		}

		try
		{
			pitch.Octave = (int)Convert.ToUInt32(octave); // Convert to UInt to ensure positivity
		}
		catch
		{
			throw new Exception($"Octave '{octave}' is not an unsigned integer.");
		}

		return pitch;
	}
}