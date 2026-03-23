namespace AbstractSyntax;

public static class AST
{
	public static int SampleRate = 48000; // TODO: Implement user-defined sample rate

	public static CustomDictionary<string, Sample> Samples = new();
	public static CustomDictionary<string, Melody> Melodies = new();
	public static Timeline TheTimeline = new();

	private static void AddToDictionary<T>(CustomDictionary<string, T> dictionary, string id, T item)
	{
		try
		{
			dictionary.Add(id, item);
		}
		catch (Exception ex) when (ex is ArgumentException || ex is ArgumentNullException)
		{
			throw new Exception($"More than one {typeof(T).Name} is using the ID '{id}'");
		}
	}
}