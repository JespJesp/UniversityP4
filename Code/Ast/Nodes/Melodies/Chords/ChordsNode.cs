using Lexing.Tokens;

namespace Ast.Nodes.Melodies.Chords;

public class ChordsNode(Node parent, bool createsNestedScope = false) : Node(parent, createsNestedScope)
{
	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.ChordsKeyword);

		while (Parser.TryConsumeIndent(2))
		{
			new ChordNode(this);
		}
	}
}

