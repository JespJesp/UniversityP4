using Lexing.Tokens;

namespace Ast.Nodes.Melodies.Samples;

public class SampleReferencesNode(Node parent, bool createsNestedScope = false) : Node(parent, createsNestedScope)
{
	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.SamplesKeyword);

	public override void CascadeParse(Parser parser)
	{
		while (parser.TryConsumeNewlineIndent(2))
		{
			new SampleReferenceNode(this);
		}
	}
}

