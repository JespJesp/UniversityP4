namespace Ast.NodeArchetypes;

public abstract class RootNode : Node
{
	public void ParseTree()
	{
		this.Parse();
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

