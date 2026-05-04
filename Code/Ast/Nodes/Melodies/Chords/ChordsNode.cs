using Lexing.Tokens;

namespace Ast.Nodes.Melodies.Chords;

public class ChordsNode(Node parent, bool createsNestedScope = false) : Node(parent, createsNestedScope)
{
	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.ChordsKeyword);

	public override void CascadeParse(Parser parser)
	{
		while (parser.TryConsumeNewlineIndent(2))
		{
			new ChordNode(this);
		}
	}
}

