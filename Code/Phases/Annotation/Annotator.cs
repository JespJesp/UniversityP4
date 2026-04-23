using Ast;
using Ast.Nodes;

namespace Phases.Annotation;

public class Annotator
{
	private List<string> _errors = new();

	public void Annotate(ProgramNode programNode)
	{
		_errors.Clear();

		CascadeAnnotate(programNode, new());

		if (_errors.Any())
		{
			throw new Exception("Semantic errors (from annotation phase):\n- " + string.Join("\n- ", _errors));
		}
	}

	private void CascadeAnnotate(Node node, SymbolTable availableSymbols)
	{
		node.SymbolTable = availableSymbols.Clone();

		try
		{
			node.Annotate(this);
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