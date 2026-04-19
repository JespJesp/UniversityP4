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
		while (parser.TryConsumeIndent(2))
		{
			parser.ParseChild(this, new ChordNode(this));
		}
	}
}

