using Lexing.Tokens;

namespace Ast.Nodes.Melodies.Samples;

public class SampleReferencesNode(Node parent, bool createsNestedScope = false) : Node(parent, createsNestedScope)
{
	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.SamplesKeyword);

		while (Parser.TryConsumeIndent(2))
		{
			new SampleReferenceNode(this);
		}
	}
}

