using Ast.NodeArchetypes;
using Phases.Parsing;
using Tokens;

namespace Ast.Nodes.Melodies.Chords;

public class ChordsNode : Node
{
	public MelodyNode MelodyNode;

	public ChordsNode(MelodyNode melodyNode)
	{
		this.MelodyNode = melodyNode;
	}

	public override void CascadeParse()
	{
		Parser.ConsumeToken(TokenType.ChordsKeyword);

		while (Parser.TryConsumeIndent(2))
		{
			Parser.ParseChild(this, new ChordNode(this));
		}
	}
}

