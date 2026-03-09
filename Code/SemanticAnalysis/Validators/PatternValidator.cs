using AST;

namespace SemanticAnalysis.Validators;

public static class PatternValidator
{
	public static void Validate(SemanticAnalyzer analyzer, Pattern pattern)
	{
		if (pattern.Samples.Count == 0)
		{
			analyzer.AddError("Pattern must have at least one sample");
		}

		ValidateLeaves(analyzer, pattern);
	}

	private static void ValidateLeaves(SemanticAnalyzer analyzer, Pattern pattern)
	{
		foreach (Sample sample in pattern.Samples)
		{
			SampleValidator.Validate(analyzer, sample);
		}
		foreach (Note note in pattern.Notes)
		{
			NoteValidator.Validate(analyzer, note);
		}
	}

}