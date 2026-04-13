using Ast.NodeArchetypes;
using Lexing.Tokens;

namespace Ast.Nodes.Melodies.Chords;

public class ChordsNode : BranchNode
{
	public MelodyNode MelodyNode;

	public ChordsNode(Node parent, MelodyNode melodyNode) : base(parent)
	{
		this.MelodyNode = melodyNode;
	}

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.ChordsKeyword);

		while (Parser.TryConsumeIndent(2))
		{
			new ChordNode(this, this);
		}
	}
}

