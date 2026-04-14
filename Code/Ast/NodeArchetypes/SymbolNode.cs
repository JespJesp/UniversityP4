using Runtime.Objects;

namespace Ast.NodeArchetypes;

public abstract class SymbolNode : BranchNode
{
	public string Id = "";

	protected sealed override void Annotate()
	{
		_symbolTable.Upsert(this);
	}
}

