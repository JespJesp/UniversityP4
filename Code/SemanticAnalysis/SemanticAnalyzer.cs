using SemanticAnalysis.Validators;
using AbstractSyntax;

namespace SemanticAnalysis;

public class SemanticAnalyzer
{
	private List<string> _errors = new List<string>();

	public void Validate()
	{
		// Reset variables
		_errors.Clear();

		ValidateRoot();

		if (_errors.Any())
		{
			throw new Exception("Semantic errors:\n" + string.Join("\n- ", _errors));
		}
	}

	public void ValidateRoot()
	{
		ASTValidator.Validate(this);
	}

	public void AddError(string message)
	{
		_errors.Add(message);
	}
}