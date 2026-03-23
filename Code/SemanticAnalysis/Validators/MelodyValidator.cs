using AbstractSyntax;

namespace SemanticAnalysis.Validators;

public static class MelodyValidator
{
	public static void Validate(SemanticAnalyzer analyzer, Melody melody)
	{
		// ID
		if (string.IsNullOrWhiteSpace(melody.Id))
		{
			analyzer.AddError("Melody ID cannot be empty");
		}

		// Samples
		if (melody.SampleIds.Count == 0)
		{
			analyzer.AddError($"Melody '{melody.Id}' must have at least one sample");
		}
		foreach (string sampleId in melody.SampleIds)
		{
			if (!AST.Samples.ContainsKey(sampleId))
			{
				analyzer.AddError($"The sample '{sampleId}' reference in melody '{melody.Id}' is undefined.");
			}
		}

		// Misc
		if (melody.Length <= 0)
		{
			analyzer.AddError("Melody length cannot be <= 0");
		}

		// Leaf validation
		foreach (Note note in melody.Notes)
		{
			NoteValidator.Validate(analyzer, note);
		}
	}
}