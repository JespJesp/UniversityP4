using Ast.Nodes.Timelines.Commands.Modifiers;
using Phases.Parsing;
using Tokens;

namespace Ast.Nodes.Timelines.Commands;

public class ModifiersNode : Node
{
	public CommandNode CommandNode;

	public ModifiersNode(CommandNode commandNode)
	{
		this.CommandNode = commandNode;
	}

	public override void CascadeParse(Parser parser)
	{
		parser.TryConsumeOptions
		(
			new()
			{
				(
					() => parser.TryConsumeToken(TokenType.Identifier, "gain"),
					() => parser.ParseChild(this, new GainNode(this))
				),
				(
					() => parser.TryConsumeToken(TokenType.Identifier, "pitch"),
					() => parser.ParseChild(this, new PitchNode(this))
				),
			},
			[
				new(TokenType.Comma)
			]
		);

		parser.ConsumeToken(TokenType.RightParentheses);
	}
}

