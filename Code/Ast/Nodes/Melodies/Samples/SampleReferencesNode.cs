using Ast.NodeArchetypes;
using Lexing.Tokens;

namespace Ast.Nodes.Melodies.Samples;

public class SampleReferencesNode : BranchNode
{
	public MelodyNode MelodyNode;

	public SampleReferencesNode(MelodyNode melodyNode)
	{
		this.MelodyNode = melodyNode;
	}

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.SamplesKeyword);

		while (Parser.TryConsumeIndent(2))
		{
			ParseChild(new SampleReferenceNode(this));
		}
	}
}

