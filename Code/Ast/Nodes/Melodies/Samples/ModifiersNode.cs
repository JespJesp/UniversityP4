using Ast.Nodes.Melodies.Samples.Modifiers;
using Phases.Parsing;
using Tokens;

namespace Ast.Nodes.Melodies.Samples;

public class ModifiersNode : Node
{
	public SampleReferenceNode SampleReferenceNode;

	public ModifiersNode(SampleReferenceNode sampleReferenceNode)
	{
		this.SampleReferenceNode = sampleReferenceNode;
	}

	public override void CascadeParse(Parser parser)
	{
		List<Func<bool>> options = new()
		{
			() => parser.TryConsumeToken(TokenType.Identifier, "attack", (value) => parser.ParseChild(this, new AttackNode(this))),
			() => parser.TryConsumeToken(TokenType.Identifier, "decay", (value) => parser.ParseChild(this, new DecayNode(this))),
			() => parser.TryConsumeToken(TokenType.Identifier, "delay", (value) => parser.ParseChild(this, new DelayNode(this))),
			() => parser.TryConsumeToken(TokenType.Identifier, "hold", (value) => parser.ParseChild(this, new HoldNode(this))),
			() => parser.TryConsumeToken(TokenType.Identifier, "release", (value) => parser.ParseChild(this, new ReleaseNode(this))),
			() => parser.TryConsumeToken(TokenType.Identifier, "sustain", (value) => parser.ParseChild(this, new SustainNode(this))),
		};
		Token[] optionSeparator =
		{
			new(TokenType.Comma)
		};
		parser.TryConsumeOptions(options, optionSeparator);

		parser.ConsumeToken(TokenType.RightParentheses);
	}
}

