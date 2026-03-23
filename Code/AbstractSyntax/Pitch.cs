using System.Text.RegularExpressions;

namespace AbstractSyntax;

public class Pitch
{
	/// <summary>
	/// Goes from C=0 to B=11.
	/// </summary>
	public int PitchClass;
	public int Octave;

	/// <summary>
	/// Note: This converts the input pitch string to lower case before processing it.
	/// For example, this pitch string could be "C5" or "c#3".
	/// </summary>
	public Pitch(string pitch)
	{
		string toLower = pitch.ToLower();
		string[] parts = Regex.Split(toLower, @"(\d+)"); // Splits the string when it reaches a digit
		string pitchClass = parts[0];
		string octave = parts[1];

		switch (pitchClass)
		{
			case "c": PitchClass = 0; break;
			case "c#" or "db": PitchClass = 1; break;
			case "d": PitchClass = 2; break;
			case "d#" or "eb": PitchClass = 3; break;
			case "e": PitchClass = 4; break;
			case "f": PitchClass = 5; break;
			case "f#" or "gb": PitchClass = 6; break;
			case "g": PitchClass = 7; break;
			case "g#" or "ab": PitchClass = 8; break;
			case "a": PitchClass = 9; break;
			case "a#" or "bb": PitchClass = 10; break;
			case "b": PitchClass = 11; break;
			default: throw new Exception($"Pitch class '{pitchClass}' is not recognized.");
		}

		try
		{
			Octave = (int)Convert.ToUInt32(octave); // Convert to UInt to ensure positivity
		}
		catch
		{
			throw new Exception($"Octave '{octave}' is not an unsigned integer.");
		}
	}
}