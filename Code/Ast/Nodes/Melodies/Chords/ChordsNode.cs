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

	public override void CascadeParse(Parser parser)
	{
		parser.ConsumeToken(TokenType.ChordsKeyword);

		while (parser.TryConsumeIndent(2))
		{
			parser.ParseChild(this, new ChordNode(this));
		}
	}
}

