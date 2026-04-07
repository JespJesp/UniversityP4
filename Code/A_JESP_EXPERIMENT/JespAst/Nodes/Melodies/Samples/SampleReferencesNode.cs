using LexicalAnalysis.Tokens;

namespace JespAst.Nodes.Melodies.Samples;

public class SampleReferencesNode(Node parent) : Node(parent)
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

