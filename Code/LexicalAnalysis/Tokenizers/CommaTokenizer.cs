using AST;

namespace SemanticAnalysis.Validators;

public static class PatternValidator
{
	public static void Validate(SemanticAnalyzer analyzer, Pattern pattern)
	{
		if (pattern.Length <= 0)
		{
			analyzer.AddError("Pattern length cannot be <= 0");
		}
		if (string.IsNullOrWhiteSpace(pattern.Name))
		{
			analyzer.AddError("Pattern name cannot be empty");
		}
		if (pattern.Samples.Count == 0)
		{
			analyzer.AddError("Pattern must have at least one sample");
		}

		// Leaf validation
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