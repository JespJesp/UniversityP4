using Ast.NodeArchetypes;
using Ast.Nodes;

namespace Validation;

public static class Validator
{
	private static List<string> _errors = new();

	public static void Validate(ProgramNode programNode)
	{
		programNode.ValidateTree();

		if (_errors.Any())
		{
			throw new Exception("Semantic errors:\n- " + string.Join("\n- ", _errors));
		}
	}

	public static void AddError(Node node, string errorMessage)
	{
		_errors.Add($"Line: '{node.Line}'. Column: '{node.Column}'. Node: '{node.GetType()}'. {errorMessage}");
	}
}