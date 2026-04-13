using Ast.NodeArchetypes;
using Lexing.Tokens;

namespace Ast.Nodes.Melodies.Samples;

public class SampleReferencesNode : BranchNode
{
	public MelodyNode MelodyNode;

	public SampleReferencesNode(Node parent, MelodyNode melodyNode) : base(parent)
	{
		this.MelodyNode = melodyNode;
	}

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.SamplesKeyword);

		while (Parser.TryConsumeIndent(2))
		{
			new SampleReferenceNode(this, this);
		}
	}
}

