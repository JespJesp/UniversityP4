using LexicalAnalysis.Tokens;

namespace JespAst.Nodes.Samples;

public class SamplesNode(Node parent) : Node(parent)
{
	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.SamplesKeyword);

		while (Parser.TryConsumeIndent(1))
		{
			new SampleDeclaration(this);
		}
	}
}

