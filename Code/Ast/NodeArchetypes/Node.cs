namespace Ast.NodeArchetypes;

public abstract class Node
{
	protected List<Node> _children = new();
	protected SymbolTable _symbolTable = new();
	private bool _createsNestedScope;
	public int ScopeDepth { get; private set; }
	public int Column { get; private set; }
	public int Line { get; private set; }

	protected T ParseChild<T>(T child, bool createsNestedScope = false) where T : Node
	{
		_children.Add(child);
		child.Column = Parser.CursorToken.Column;
		child.Line = Parser.CursorToken.Line;
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

	protected void CascadeAnnotate(SymbolTable availableSymbols)
	{
		this._symbolTable = availableSymbols.Clone();
		Annotate();

		SymbolTable childrensSymbols = this._symbolTable;
		foreach (Node child in _children)
		{
			child.CascadeAnnotate(childrensSymbols);
			childrensSymbols = child._symbolTable; // Inherit symbols from older sibling
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
			throw new Exception($"Column: {this.Column}. Line: {this.Line}. Node: {this.GetType()}. {exception.Message}");
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

