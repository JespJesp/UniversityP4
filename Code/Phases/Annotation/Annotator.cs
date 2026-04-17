using Ast;
using Ast.NodeArchetypes;
using Ast.Nodes;

namespace Phases.Annotation;

public static class Annotator
{
	private static List<string> _errors = new();

	public static void Annotate(ProgramNode programNode)
	{
		CascadeAnnotate(programNode, new());

		if (_errors.Any())
		{
			throw new Exception("Annotation errors:\n- " + string.Join("\n- ", _errors));
		}
	}

	private static void CascadeAnnotate(Node node, SymbolTable availableSymbols)
	{
		node.SymbolTable = availableSymbols.Clone();

		try
		{
			node.Annotate();
		}
		catch (Exception exception)
		{
			_errors.Add($"Line: '{node.Line}'. Column: '{node.Column}'. Node: '{node.GetType()}'. {exception.Message}");
		}

		SymbolTable childrensSymbols = node.SymbolTable;
		foreach (Node child in node.Children)
		{
			CascadeAnnotate(child, childrensSymbols);
			childrensSymbols = child.SymbolTable; // Inherit symbols from older sibling
		}
	}
}