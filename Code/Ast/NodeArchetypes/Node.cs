namespace Ast.NodeArchetypes;

public abstract class Node
{
	protected List<Node> _children = new();
	protected SymbolTable _symbolTable;
	private bool _createsNestedScope;
	public int ScopeDepth { get; private set; }

	protected T ParseChild<T>(T child, bool createsNestedScope = false) where T : Node
	{
		_children.Add(child);
		child._createsNestedScope = createsNestedScope;

		if (this._createsNestedScope)
		{
			child.ScopeDepth = this.ScopeDepth + 1;
		}
		else
		{
			child.ScopeDepth = this.ScopeDepth;
		}

		try
		{
			child.Parse();
		}
		catch (Exception exception)
		{
			Parser.AddError($"Node type: {child.GetType()}. {exception.Message}");
		}

		return child;
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

