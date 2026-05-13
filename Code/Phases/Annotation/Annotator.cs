using Ast;
using Ast.Nodes;

namespace Phases.Annotation;

public class Annotator
{
	private List<string> _errors = new();

	public void Annotate(FileNode rootNode)
	{
		_errors.Clear();

		CascadeAnnotate(rootNode, new());

		if (_errors.Any())
		{
			throw new Exception("Semantic errors (from annotation phase):\n- " + string.Join("\n- ", _errors));
		}
	}

	private void CascadeAnnotate(Node node, SymbolTable availableSymbols)
	{
		node.SymbolTable = availableSymbols;

		try
		{
			node.Annotate(this);
		}
		catch (Exception exception)
		{
			_errors.Add($"{node.Location}. Node: '{node.GetType()}'. {exception.Message}");
		}

		SymbolTable childrensSymbols = node.SymbolTable;

		// Ensure that nested children do not affect the symbol tables of higher levels
		if (node.CreatesNestedScope)
		{
			childrensSymbols = childrensSymbols.Clone();
		}

		foreach (Node child in node.Children)
		{
			CascadeAnnotate(child, childrensSymbols);
		}
	}
}