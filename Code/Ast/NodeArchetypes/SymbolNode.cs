using Runtime.Objects;

namespace Ast.NodeArchetypes;

public abstract class SymbolNode : BranchNode
{
	public string Id = "";

	protected SymbolNode(Node parent, bool createsNestedScope = false) : base(parent, createsNestedScope)
	{
	}

	protected sealed override void Annotate()
	{
		_symbolTable.Upsert(this);
	}
}

