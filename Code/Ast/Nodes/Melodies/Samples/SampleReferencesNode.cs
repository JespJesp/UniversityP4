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

	public override void CascadeParse(Parser parser)
	{
		parser.ConsumeToken(TokenType.SamplesKeyword);

		while (parser.TryConsumeIndent(2))
		{
			parser.ParseChild(this, new SampleReferenceNode(this));
		}
	}
}

