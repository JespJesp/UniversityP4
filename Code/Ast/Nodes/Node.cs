using Ast.Tables;
using Runtime;

namespace Ast.Nodes;

public abstract class Node
{
	protected List<Node> _children = new();
	public int Scope { get; }
	private bool _createsNestedScope;

	public Node(Node parent, bool createsNestedScope = false)
	{
		// TODO: Parent is only null if it is the program node (so the root node)
		// so, I should rewrite this to require a parent and then have ProgramNode
		// have its own separate logic.
		if (parent is not null)
		{
			parent._children.Add(this);
			if (parent._createsNestedScope)
			{
				this.Scope = parent.Scope + 1;
			}
			else
			{
				this.Scope = parent.Scope;
			}
		}
		else
		{
			this.Scope = 0;
		}

		this._createsNestedScope = createsNestedScope;

		try
		{
			Parse();
		}
		catch (Exception exception)
		{
			Parser.AddSyntaxError($"Node type: {this.GetType()}. {exception.Message}");
		}
	}

	protected abstract void Parse();

	protected virtual void Annotate(NodeTable ancestors, SemanticSymbolTable symbols) { }
	public void CascadeAnnotate(NodeTable ancestors, SemanticSymbolTable symbols)
	{
		Annotate(ancestors, symbols);

		// TODO: Rewrite this comment explanation to be better.
		// Letting the children to work on and modify a clones
		// ensures that they don't affect nodes outside of their scope.

		var childrensAncestors = ancestors.Clone();
		childrensAncestors.Upsert(this);

		var childrensSymbols = symbols;
		if (_createsNestedScope)
		{
			childrensSymbols = symbols.Clone();
		}

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

		// TODO: Write comment explanation of why we use clone here

		var childrensAncestors = ancestors.Clone();
		childrensAncestors.Upsert(this);

		var childrensVariables = variables;
		if (_createsNestedScope)
		{
			childrensVariables = variables.Clone();
		}

		foreach (Node child in _children)
		{
			child.CascadeEvaluate(childrensAncestors, childrensVariables);
		}
	}
}

