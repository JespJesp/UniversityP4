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
		parser.TryConsumeOptions
		(
			new() 
			{
				(
					() => parser.TryConsumeToken(TokenType.Identifier, "attack"),
					() => parser.ParseChild(this, new AttackNode(this))
				),
				(
					() => parser.TryConsumeToken(TokenType.Identifier, "decay"),
					() => parser.ParseChild(this, new DecayNode(this))
				),
				(
					() => parser.TryConsumeToken(TokenType.Identifier, "delay"),
					() => parser.ParseChild(this, new DelayNode(this))
				),
				(
					() => parser.TryConsumeToken(TokenType.Identifier, "hold"),
					() => parser.ParseChild(this, new HoldNode(this))
				),
				(
					() => parser.TryConsumeToken(TokenType.Identifier, "release"),
					() => parser.ParseChild(this, new ReleaseNode(this))
				),
				(
					() => parser.TryConsumeToken(TokenType.Identifier, "sustain"),
					() => parser.ParseChild(this, new SustainNode(this))
				),
			},
			[
				new(TokenType.Comma)
			]
		);

		parser.ConsumeToken(TokenType.RightParentheses);
	}
}