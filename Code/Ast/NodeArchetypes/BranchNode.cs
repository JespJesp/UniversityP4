namespace Ast.NodeArchetypes;

public abstract class BranchNode : Node
{
	public BranchNode(Node parent, bool createsNestedScope = false) : base(parent, createsNestedScope)
	{
	}
}

