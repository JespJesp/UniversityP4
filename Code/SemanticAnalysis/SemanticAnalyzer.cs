using SemanticAnalysis.Validators;
using AST;

namespace SemanticAnalysis;

public class SemanticAnalyzer
{
	private List<string> _errors = new List<string>();

	public void Validate(Song song)
	{
		// Reset variables
		_errors.Clear();

		ValidateRoot(song);

		if (_errors.Any())
		{
			throw new Exception("Semantic errors:\n" + string.Join("\n- ", _errors));
		}
	}

	public void ValidateRoot(Song song)
	{
		SongValidator.Validate(this, song);
	}

	public void AddError(string message)
	{
		_errors.Add(message);
	}
}