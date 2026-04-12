using Ast.Nodes;

namespace Ast;

public static class Annotator
{
	private static List<string> _semanticErrors = new();

	public static void AnnotateTree(ProgramNode programNode)
	{
		programNode.CascadeAnnotate(new(), new());

		if (_semanticErrors.Any())
		{
			throw new Exception("Semantic errors:\n- " + string.Join("\n- ", _semanticErrors));
		}
	}

	public static void AddError(Node node, string errorMessage)
	{
		_semanticErrors.Add($"Node: '{node.GetType()}'. {errorMessage}");
	}
}