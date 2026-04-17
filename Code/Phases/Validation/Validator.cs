using Ast.NodeArchetypes;
using Ast.Nodes;

namespace Phases.Validation;

public class Validator
{
	private List<string> _errors = new();

	public void Validate(ProgramNode programNode)
	{
		CascadeValidate(programNode);

		if (_errors.Any())
		{
			throw new Exception("Semantic errors:\n- " + string.Join("\n- ", _errors));
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
			_errors.Add($"Line: '{node.Line}'. Column: '{node.Column}'. Node: '{node.GetType()}'. {exception.Message}");
		}

		foreach (Node child in node.Children)
		{
			CascadeValidate(child);
		}
	}
}