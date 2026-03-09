using AST;

namespace SemanticAnalysis.Validators;

public static class SongValidator
{
	public static void Validate(SemanticAnalyzer analyzer, Song song)
	{
		foreach (Pattern pattern in song.Patterns)
		{
			PatternValidator.Validate(analyzer, pattern);
		}
	}
}