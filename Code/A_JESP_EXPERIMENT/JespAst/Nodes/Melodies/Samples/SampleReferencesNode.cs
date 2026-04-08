using LexicalAnalysis.Tokens;

namespace JespAst.Nodes.Melodies.Samples;

public class SampleReferencesNode(Node parent, bool createsNestedScope = false) : Node(parent, createsNestedScope)
{
	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.SampleKeyword);

		while (Parser.TryConsumeIndent(2))
		{
			new SampleReferenceNode(this);
		}
	}
}

