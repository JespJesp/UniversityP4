using AST;

namespace SemanticAnalysis.Validators;

public static class PatternValidator
{
	public static void Validate(SemanticAnalyzer analyzer, Pattern pattern)
	{
		// ID
		if (string.IsNullOrWhiteSpace(pattern.Id))
		{
			analyzer.AddError("Pattern ID cannot be empty");
		}
		if (pattern.ParentSong.Patterns.FindAll(aPattern => aPattern.Id == pattern.Id).Count > 1)
		{
			analyzer.AddError($"More than one pattern is using the ID '{pattern.Id}'");
		}

		// Samples
		if (pattern.SampleIds.Count == 0)
		{
			analyzer.AddError($"Pattern '{pattern.Id}' must have at least one sample");
		}
		foreach (string sampleId in pattern.SampleIds)
		{
			try
			{
				pattern.ParentSong.GetSample(sampleId);
			}
			catch
			{
				analyzer.AddError($"The sample '{sampleId}' in pattern '{pattern.Id}' is undefined.");
			}
		}

		// Misc
		if (pattern.Length <= 0)
		{
			analyzer.AddError("Pattern length cannot be <= 0");
		}

		// Leaf validation
		foreach (Note note in pattern.Notes)
		{
			NoteValidator.Validate(analyzer, note);
		}
	}
}