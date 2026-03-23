namespace AbstractSyntax;

public static class AST
{
	public static int SampleRate = 48000; // TODO: Implement user-defined sample rate

	public static CustomDictionary<string, Sample> Samples = new();
	public static CustomDictionary<string, Melody> Melodies = new();
	public static CustomDictionary<string, Pattern> Patterns = new();
	public static Timeline TheTimeline = new();
}