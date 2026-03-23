using AbstractSyntax;

namespace SemanticAnalysis.Validators;

public static class SongValidator
{
	public static void Validate(SemanticAnalyzer analyzer)
	{
		// Leaf validation
		foreach (Sample sample in AST.Samples.Values)
		{
			SampleValidator.Validate(analyzer, sample);
		}
		foreach (Melody melody in AST.Melodies.Values)
		{
			MelodyValidator.Validate(analyzer, melody);
		}
		TimelineValidator.Validate(analyzer, AST.TheTimeline);
	}
}