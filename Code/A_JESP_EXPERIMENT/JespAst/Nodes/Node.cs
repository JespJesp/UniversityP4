using JespRuntime;

namespace JespAst.Nodes;

public abstract class Node
{
	protected List<Node> _children = new();
	//TODO: Implement syntax errors
	protected List<Exception> _semanticErrors = new();

	public Node(Node parent)
	{
		parent._children.Add(this);
	}

	protected void AddSemanticError(string message)
	{
		_semanticErrors.Add(new Exception(message));
	}

	protected abstract void Parse();
	public void CascadeParse()
	{
		Parse();
		foreach (Node child in _children)
		{
			child.Parse();
		}
	}

	protected virtual void Annotate(HashSet<(Type, object)> parentNodesTable, HashSet<(Type, string)> localSymbolTable) { }
	public void CascadeAnnotate(HashSet<(Type, object)> parentNodesTable, HashSet<(Type, string)> localSymbolTable)
	{
		Annotate(localSymbolTable);

		// TODO: Rewrite this comment explanation to be better.
		// Letting the children to work on and modify a clone of the symbol table
		// ensures that they don't affect nodes outside of their scope.
		var childrensLocalSymbolTable = new HashSet<(Type, string)>(localSymbolTable);
		foreach (Node child in _children)
		{
			child.CascadeAnnotate(childrensLocalSymbolTable);
		}
	}

	protected virtual void Evaluate(HashSet<(Type, object)> parentNodesTable, HashSet<(Type, string, object)> localVariables) { }
	public void CascadeEvaluate(HashSet<(Type, object)> parentNodesTable, HashSet<(Type, string, object)> localVariables)
	{
		Evaluate(localVariables);

		// TODO: Write comment explanation of why we use clone here
		var childrensLocalVariables = localVariables.Clone();
		foreach (Node child in _children)
		{
			child.CascadeEvaluate(childrensLocalVariables);
		}
	}
}

