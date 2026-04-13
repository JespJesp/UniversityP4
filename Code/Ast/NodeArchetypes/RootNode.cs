namespace Ast.NodeArchetypes;

public abstract class RootNode : Node
{
	public RootNode() : base(null, false)
	{
	}

	public void AnnotateTree()
	{
		this.CascadeAnnotate(new());
	}

	public void ValidateTree()
	{
		this.CascadeValidate();
	}

	public void EvaluateTree()
	{
		this.CascadeEvaluate();
	}
}

