using Ast.Tables;

namespace Ast.NodeArchetypes;

public abstract class Node
{
	protected List<Node> _children = new();
	public int ScopeDepth { get; }
	private bool _createsNestedScope;

	/// <summary>
	/// This constructor automatically parses the node.
	/// </summary>
	public Node(Node? parent, bool createsNestedScope)
	{
		this._createsNestedScope = createsNestedScope;

		// If parent is null, then this node is the root node of the tree
		if (parent is null)
		{
			this.ScopeDepth = 0;
		}
		else
		{
			parent._children.Add(this);
			if (parent._createsNestedScope)
			{
				this.ScopeDepth = parent.ScopeDepth + 1;
			}
			else
			{
				this.ScopeDepth = parent.ScopeDepth;
			}
		}

		// Parse the node
		try
		{
			Parse();
		}
		catch (Exception exception)
		{
			Parser.AddError($"Node type: {this.GetType()}. {exception.Message}");
		}
	}

	public void CascadeValidate(SemanticSymbolTable symbols)
	{
		Validate(symbols);

		// We clone the inherited table and let the children work with the clone in cases
		// where we don't want sibling nodes to affect their ancestors, cousins, and uncles/aunts.
		var childrensSymbols = _createsNestedScope ? symbols.Clone() : symbols;

		foreach (Node child in _children)
		{
			child.CascadeValidate(childrensSymbols);
		}
	}

	public void CascadeEvaluate(RuntimeVariableTable variables)
	{
		try
		{
			Evaluate(variables);
		}
		catch (Exception exception)
		{
			throw new Exception($"Node: {this.GetType()}. {exception.Message}");
		}

		// We clone the inherited table and let the children work with the clone in cases
		// where we don't want sibling nodes to affect their ancestors, cousins, and uncles/aunts.
		var childrensVariables = _createsNestedScope ? variables.Clone() : variables;

		foreach (Node child in _children)
		{
			child.CascadeEvaluate(childrensVariables);
		}
	}

	protected abstract void Parse();
	protected virtual void Validate(SemanticSymbolTable symbols) { }
	protected virtual void Evaluate(RuntimeVariableTable variables) { }
}

