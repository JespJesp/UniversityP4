using AST;

namespace SemanticAnalysis.Validators;

public static class SampleValidator
{
	public static void Validate(SemanticAnalyzer analyzer, Sample sample)
	{
		// ID
		if (string.IsNullOrWhiteSpace(sample.Id))
		{
			analyzer.AddError("Sample ID cannot be empty");
		}
		if (sample.ParentSong.Samples.FindAll(aSample => aSample.Id == sample.Id).Count > 1)
		{
			analyzer.AddError($"More than one sample is using the ID '{sample.Id}'");
		}

		// File path
		if (string.IsNullOrWhiteSpace(sample.FilePath))
		{
			analyzer.AddError($"Sample '{sample.Id}' file path name cannot be empty");
		}
		if (!sample.FilePath.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
		{
			analyzer.AddError($"Sample '{sample.Id}' with file path '{sample.FilePath}' must be a .wav file");
		}
	}
}