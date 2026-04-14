using Ast.NodeArchetypes;
using Lexing.Tokens;

namespace Ast.Nodes.Melodies.Chords;

public class ChordsNode : BranchNode
{
	public MelodyNode MelodyNode;

	public ChordsNode(MelodyNode melodyNode)
	{
		this.MelodyNode = melodyNode;
	}

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.ChordsKeyword);

		while (Parser.TryConsumeIndent(2))
		{
			ParseChild(new ChordNode(this));
		}
	}
}

