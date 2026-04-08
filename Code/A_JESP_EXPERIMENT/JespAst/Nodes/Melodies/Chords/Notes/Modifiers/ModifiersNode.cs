using LexicalAnalysis.Tokens;

namespace JespAst.Nodes.Melodies.Chords.Notes.Modifiers;

public class ModifiersNode(Node parent, bool createsNestedScope = false) : Node(parent, createsNestedScope)
{
	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.LeftParentheses);

		Dictionary<TokenType, Action> options = new()
		{
			{
				TokenType.GainKeyword,
				() => { new GainNode(this); }
			},
			{
				TokenType.PanKeyword,
				() => { new PanNode(this); }
			}
		};
		Token optionSeparator = new(TokenType.Comma);
		Parser.TryConsumeUniqueOptions(options, optionSeparator);

		Parser.ConsumeToken(TokenType.RightParentheses);
	}
}

