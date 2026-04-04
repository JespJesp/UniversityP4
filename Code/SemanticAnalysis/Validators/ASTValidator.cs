using AbstractSyntax;

namespace SemanticAnalysis.Validators;

public static class ASTValidator
{
	public static void Validate(SemanticAnalyzer analyzer)
	{
		// Leaf validation
		foreach (Sample sample in RuntimeEnvironment.Samples.Values)
		{
			SampleValidator.Validate(analyzer, sample);
		}
		foreach (Melody melody in RuntimeEnvironment.Melodies.Values)
		{
			MelodyValidator.Validate(analyzer, melody);
		}
		foreach (Pattern pattern in RuntimeEnvironment.Patterns.Values)
		{
			PatternValidator.Validate(analyzer, pattern);
		}
		TimelineValidator.Validate(analyzer, RuntimeEnvironment.TheTimeline);
	}
}