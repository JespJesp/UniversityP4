using Runtime;

namespace SemanticAnalysis.Validators;

public static class ASTValidator
{
	public static void Validate(SemanticAnalyzer analyzer)
	{
		// Leaf validation
		foreach (Sample sample in Runtime.Environment.Samples.Values)
		{
			SampleValidator.Validate(analyzer, sample);
		}
		foreach (Melody melody in Runtime.Environment.Melodies.Values)
		{
			MelodyValidator.Validate(analyzer, melody);
		}
		foreach (Pattern pattern in Runtime.Environment.Patterns.Values)
		{
			PatternValidator.Validate(analyzer, pattern);
		}
		TimelineValidator.Validate(analyzer, Runtime.Environment.TheTimeline);
	}
}