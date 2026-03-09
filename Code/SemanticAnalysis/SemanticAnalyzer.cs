using System.Text.RegularExpressions;
using SemanticAnalysis.Validators;
using AST;

namespace SemanticAnalysis;

public class SemanticAnalyzer
{
	protected List<string> Errors = new List<string>();

	public void Validate(Song song)
	{
		// Reset variables
		Errors.Clear();

		ValidateRoot(song);

		if (Errors.Any())
		{
			throw new Exception("Semantic errors:\n" + string.Join("\n", Errors));
		}
	}

	public void ValidateRoot(Song song)
	{
		SongValidator.Validate(this, song);
	}

	public void AddError(string message)
	{
		Errors.Add(message);
	}
}