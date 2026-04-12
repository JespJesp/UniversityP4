
using Ast.Tables;

namespace Ast.Nodes;

public abstract class SymbolNode(Node parent, bool createsNestedScope = false) : Node(parent, createsNestedScope)
{
	public string Id = "";

	protected sealed override void Annotate(NodeTable ancestors, SemanticSymbolTable symbols)
	{
		symbols.Add(this, Id);
		AdditionalAnnotation(ancestors, symbols);
	}

	/// <summary>
	/// SymbolNodes automatically add their symbol to the symbol table. 
	/// If you want to do more annotation, use this method for that.
	/// </summary>
	protected abstract void AdditionalAnnotation(NodeTable ancestors, SemanticSymbolTable symbols);
}

