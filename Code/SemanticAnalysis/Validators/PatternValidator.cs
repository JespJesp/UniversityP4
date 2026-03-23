using AbstractSyntax;

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

		// Children
		foreach (string patternOrMelodyId in pattern.PatternAndMelodyIds)
		{
			if (!AST.Patterns.ContainsKey(patternOrMelodyId) && !AST.Melodies.ContainsKey(patternOrMelodyId))
			{
				analyzer.AddError($"The reference '{patternOrMelodyId}' in pattern '{pattern.Id}' is undefined.");
			}
		}

		// Length
		if (pattern.Length <= 0)
		{
			analyzer.AddError("Pattern length cannot be <= 0");
		}
	}
}