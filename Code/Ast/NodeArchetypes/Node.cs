using Parsing;
using Annotation;
using Validation;
using Evaluation;

namespace Ast.NodeArchetypes;

public abstract class Node
{
	public int ScopeDepth { get; private set; }
	public int Column { get; private set; }
	public int Line { get; private set; }

	protected List<Node> _children = new();
	protected SymbolTable _symbolTable = new();

	private bool _createsNestedScope;

	protected T ParseChild<T>(T child, bool createsNestedScope = false) where T : BranchNode
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
			Parser.AddErrorAndSkipLine(child, exception.Message);
		}

		return child;
	}

	protected void CascadeAnnotate(SymbolTable availableSymbols)
	{
		this._symbolTable = availableSymbols.Clone();

		try
		{
			Annotate();
		}
		catch (Exception exception)
		{
			Annotator.AddError(this, exception.Message);
		}

		SymbolTable childrensSymbols = this._symbolTable;
		foreach (Node child in _children)
		{
			child.CascadeAnnotate(childrensSymbols);
			childrensSymbols = child._symbolTable; // Inherit symbols from older sibling
		}
	}

	protected void CascadeValidate()
	{
		try
		{
			Validate();
		}
		catch (Exception exception)
		{
			Validator.AddError(this, exception.Message);
		}

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
			Evaluator.ThrowError(this, exception.Message);
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

