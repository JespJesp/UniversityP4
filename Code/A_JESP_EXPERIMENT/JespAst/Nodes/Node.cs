using JespAst.Tables;
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

	protected virtual void Annotate(NodeTable localNodes, SymbolTable localSymbols) { }
	public void CascadeAnnotateChildren(NodeTable localNodes, SymbolTable localSymbols)
	{
		foreach (Node child in _children)
		{
			child.Annotate(localNodes, localSymbols);
		}
		
		// TODO: Rewrite this comment explanation to be better.
		// Letting the children to work on and modify a clones
		// ensures that they don't affect nodes outside of their scope.

		var childrensLocalNodes = localNodes.Clone();
		childrensLocalNodes.Upsert(this);
		var childrensLocalSymbols = localSymbols.Clone();
		
		foreach (Node child in _children)
		{
			child.CascadeAnnotateChildren(childrensLocalNodes, childrensLocalSymbols);
		}
	}

	protected virtual void Evaluate(NodeTable localNodes, VariableTable localVariables) { }
	public void CascadeEvaluateChildren(NodeTable localNodes, VariableTable localVariables)
	{
		foreach (Node child in _children)
		{
			child.Evaluate(localNodes, localVariables);
		}

		// TODO: Write comment explanation of why we use clone here

		var childrensLocalNodes = localNodes.Clone();
		childrensLocalNodes.Upsert(this);
		var childrensLocalVariables = localVariables.Clone();

		foreach (Node child in _children)
		{
			child.CascadeEvaluateChildren(childrensLocalNodes, childrensLocalVariables);
		}
	}
}

