using Ast.Tables;
using Runtime;

namespace Ast.Nodes;

public abstract class Node
{
	protected List<Node> _children = new();
	public int ScopeDepth { get; }
	private bool _createsNestedScope;

	/// <summary>
	/// This constructor automatically sets up parent-child relations and parses the node.
	/// </summary>
	public Node(Node? parent, bool createsNestedScope = false)
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

		try
		{
			Parse();
		}
		catch (Exception exception)
		{
			Parser.AddError($"Node type: {this.GetType()}. {exception.Message}");
		}
	}

	protected abstract void Parse();

	protected virtual void Annotate(NodeTable ancestors, SemanticSymbolTable symbols) { }
	public void CascadeAnnotate(NodeTable ancestors, SemanticSymbolTable symbols)
	{
		Annotate(ancestors, symbols);

		// We clone the inherited tables and let the children work with the clone in cases
		// where we don't want sibling nodes to affect their ancestors, cousins, and uncles/aunts.
		var childrensSymbols = _createsNestedScope ? symbols.Clone() : symbols;
		var childrensAncestors = ancestors.Clone();
		childrensAncestors.Upsert(this);

		foreach (Node child in _children)
		{
			child.CascadeAnnotate(childrensAncestors, childrensSymbols);
		}
	}

	protected virtual void Evaluate(NodeTable ancestors, RuntimeVariableTable variables) { }
	public void CascadeEvaluate(NodeTable ancestors, RuntimeVariableTable variables)
	{
		try
		{
			Evaluate(ancestors, variables);
		}
		catch (Exception exception)
		{
			throw new Exception($"Node: {this.GetType()}. {exception.Message}");
		}

		// We clone the inherited tables and let the children work with the clone in cases
		// where we don't want sibling nodes to affect their ancestors, cousins, and uncles/aunts.
		var childrensVariables = _createsNestedScope ? variables.Clone() : variables;
		var childrensAncestors = ancestors.Clone();
		childrensAncestors.Upsert(this);

		foreach (Node child in _children)
		{
			child.CascadeEvaluate(childrensAncestors, childrensVariables);
		}
	}
}

