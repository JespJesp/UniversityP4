using AST;

namespace SemanticAnalysis.Validators;

public static class SongValidator
{
	public static void Validate(SemanticAnalyzer analyzer, Song song)
	{
		// Leaf validation
		foreach (Sample sample in song.Samples)
		{
			SampleValidator.Validate(analyzer, sample);
		}
		foreach (Pattern pattern in song.Patterns)
		{
			PatternValidator.Validate(analyzer, pattern);
		}
		TimelineValidator.Validate(analyzer, song.TheTimeline);
	}
}