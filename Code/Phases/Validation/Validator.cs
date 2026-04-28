using Ast;
using Ast.Nodes;

namespace Phases.Validation;

public class Validator
{
	private List<string> _errors = new();

	public void Validate(ProgramNode programNode)
	{
		_errors.Clear();

		CascadeValidate(programNode);

		if (_errors.Any())
		{
			throw new Exception("Semantic errors (from validation phase):\n- " + string.Join("\n- ", _errors));
		}
	}

	private void CascadeValidate(Node node)
	{
		try
		{
			node.Validate(this);
		}
		catch (Exception exception)
		{
			_errors.Add($"{node.Location}. Node: '{node.GetType()}'. {exception.Message}");
		}

		foreach (Node child in node.Children)
		{
			CascadeValidate(child);
		}
	}
}