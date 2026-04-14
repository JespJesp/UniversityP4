using Ast.NodeArchetypes;
using Ast.Nodes;

namespace Ast;

public static class Annotator
{
	private static List<string> _annotationErrors = new();

	public static void Annotate(ProgramNode programNode)
	{
		programNode.AnnotateTree();

		if (_annotationErrors.Any())
		{
			throw new Exception("Annotation errors:\n- " + string.Join("\n- ", _annotationErrors));
		}
	}

	public static void AddError(Node node, string errorMessage)
	{
		_annotationErrors.Add($"Line: '{node.Line}'. Column: '{node.Column}'. Node: '{node.GetType()}'. {errorMessage}");
	}
}