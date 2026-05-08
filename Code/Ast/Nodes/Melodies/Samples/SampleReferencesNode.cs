using Phases.Parsing;

namespace Ast.Nodes.Melodies.Samples;

public class SampleReferencesNode : Node
{
	public MelodyNode MelodyNode;

	public SampleReferencesNode(MelodyNode melodyNode)
	{
		this.MelodyNode = melodyNode;
	}

	public override void CascadeParse(Parser parser)
	{
		while (parser.TryConsumeNewlineIndent(2))
		{
			parser.ParseChild(this, new SampleReferenceNode(this));
		}
	}
}

