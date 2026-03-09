using AST;

namespace SemanticAnalysis.Validators;

public static class SampleValidator
{
	public static void Validate(SemanticAnalyzer analyzer, Sample sample)
	{
		if (string.IsNullOrWhiteSpace(sample.FileName))
		{
			analyzer.AddError("Sample filename cannot be empty");
		}

		if (!sample.FileName.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
		{
			analyzer.AddError($"Sample file {sample.FileName} should be a .wav file");
		}
	}
}