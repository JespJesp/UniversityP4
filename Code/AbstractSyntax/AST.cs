namespace AbstractSyntax;

public static class AST
{
	public static int SampleRate = 48000; // TODO: Implement user-defined sample rate
	public static int BeatsPerMinute = 120; // TODO: Implement user-defined BPM

	// Time signature
	/// <summary>
	/// The division of a whole note. For example, if = 4, then a note represents a quarter-note.
	/// </summary>
	public static int BeatNoteValue = 4; // TODO: Implement user-defined time signature
	public static int BeatsPerBar = 4; // TODO: This doesn't do anything at the moment.

	public static CustomDictionary<string, Sample> Samples = new();
	public static CustomDictionary<string, Melody> Melodies = new();
	public static CustomDictionary<string, Pattern> Patterns = new();
	public static Timeline TheTimeline = new();
}