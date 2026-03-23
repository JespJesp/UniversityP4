using AbstractSyntax;

namespace SemanticAnalysis.Validators;

public static class ASTValidator
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
		foreach (Pattern pattern in AST.Patterns.Values)
		{
			PatternValidator.Validate(analyzer, pattern);
		}
		TimelineValidator.Validate(analyzer, AST.TheTimeline);
	}
}