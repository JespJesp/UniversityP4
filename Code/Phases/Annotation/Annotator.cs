using Ast.NodeArchetypes;
using Ast.Nodes;

namespace Phases.Annotation;

public static class Annotator
{
	private static List<string> _errors = new();

	public static void Annotate(ProgramNode programNode)
	{
		programNode.AnnotateTree();

		if (_errors.Any())
		{
			throw new Exception("Annotation errors:\n- " + string.Join("\n- ", _errors));
		}
	}

	public static void AddError(Node node, string errorMessage)
	{
		_errors.Add($"Line: '{node.Line}'. Column: '{node.Column}'. Node: '{node.GetType()}'. {errorMessage}");
	}
}