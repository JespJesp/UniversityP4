namespace AST;

public class Song
{
	public int SampleRate = 48000; // TODO: Implement user-defined sample rate

	public List<Sample> Samples = new();
	public List<Pattern> Patterns = new();
	public Timeline TheTimeline = new();

	/// <summary>
	/// Throws exception if no sample in the song has the specified id.
	/// </summary>
	public Sample GetSample(string id)
	{
		Sample? sample = Samples.Find(sample => sample.Id == id);

		if (sample == null)
		{
			throw new Exception($"Could not get sample '{id}', because it is undefined.");
		}

		return sample;
	}

	/// <summary>
	/// Throws exception if no pattern in the song has the specified id.
	/// </summary>
	public Pattern GetPattern(string id)
	{
		Pattern? pattern = Patterns.Find(pattern => pattern.Id == id);

		if (pattern == null)
		{
			throw new Exception($"Could not get pattern '{id}', because it is undefined.");
		}

		return pattern;
	}
}