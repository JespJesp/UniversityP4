using Ast.NodeArchetypes;
using Phases.Parsing;
using Tokens;

namespace Ast.Nodes.Melodies.Samples;

public class SampleReferencesNode : Node
{
	public MelodyNode MelodyNode;

	public SampleReferencesNode(MelodyNode melodyNode)
	{
		this.MelodyNode = melodyNode;
	}

	public override void CascadeParse()
	{
		Parser.ConsumeToken(TokenType.SamplesKeyword);

		while (Parser.TryConsumeIndent(2))
		{
			Parser.ParseChild(this, new SampleReferenceNode(this));
		}
	}
}

