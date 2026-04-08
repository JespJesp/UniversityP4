using Ast.Tables;
using Runtime;

namespace Ast.Nodes;

public abstract class Node
{
	protected List<Node> _children = new();
	bool _createsNestedScope;

	public Node(Node parent, bool createsNestedScope = false)
	{
		parent?._children.Add(this);
		this._createsNestedScope = createsNestedScope;
		try
		{
			Console.WriteLine("Parsing node: " + this.GetType()); // TODO: REMOVE; FOR DEBUGGING
			Parse();
		}
		catch (Exception exception)
		{
			Parser.AddSyntaxError($"Node type: {this.GetType()}. {exception.Message}");
		}
	}

	protected abstract void Parse();

	protected virtual void Annotate(NodeTable ancestors, SymbolTable symbols) { }
	public void CascadeAnnotate(NodeTable ancestors, SymbolTable symbols)
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

	protected virtual void Evaluate(NodeTable ancestors, VariableTable variables) { }
	public void CascadeEvaluate(NodeTable ancestors, VariableTable variables)
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

