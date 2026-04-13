namespace Ast.NodeArchetypes;

public abstract class Node
{
	protected List<Node> _children = new();
	protected SymbolTable _symbolTable;
	private bool _createsNestedScope;
	public int ScopeDepth { get; }

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

	protected void CascadeAnnotate(SymbolTable symbols)
	{
		this._symbolTable = symbols;
		// We clone the inherited table and let the children work with the clone in cases
		// where we don't want sibling nodes to affect their ancestors, cousins, and uncles/aunts.
		var childrensSymbols = _createsNestedScope ? symbols.Clone() : symbols;

		Annotate();
		foreach (Node child in _children)
		{
			child.CascadeAnnotate(childrensSymbols);
		}
	}

	protected void CascadeValidate()
	{
		Validate();
		foreach (Node child in _children)
		{
			child.CascadeValidate();
		}
	}

	protected void CascadeEvaluate()
	{
		try
		{
			Evaluate();
		}
		catch (Exception exception)
		{
			throw new Exception($"Node: {this.GetType()}. {exception.Message}");
		}

		foreach (Node child in _children)
		{
			child.CascadeEvaluate();
		}
	}

	protected abstract void Parse();
	protected virtual void Annotate() { }
	protected virtual void Validate() { }
	protected virtual void Evaluate() { }
}

