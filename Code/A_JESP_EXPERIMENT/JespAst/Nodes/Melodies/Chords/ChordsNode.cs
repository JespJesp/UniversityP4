using JespRuntime.Nodes;
using LexicalAnalysis.Tokens;

namespace JespAst.Nodes.Melodies.Chords;

public class ChordsNode(Node parent) : Node(parent)
{
	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.NotesKeyword);

		while (Parser.TryConsumeIndent(2))
		{
			new ChordNode(this);
		}
	}
}

