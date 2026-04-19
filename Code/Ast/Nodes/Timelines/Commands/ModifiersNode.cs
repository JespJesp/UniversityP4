using Ast.Nodes.Timelines.Commands.Modifiers;
using Phases.Evaluation;
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
		List<Func<bool>> options = new() 
		{
			() => parser.TryConsumeToken(TokenType.Identifier, "gain", (value) => parser.ParseChild(this, new GainNode(this))),
			() => parser.TryConsumeToken(TokenType.Identifier, "pitch", (value) => parser.ParseChild(this, new PitchNode(this))),
		};
		Token[] optionSeparator =
		{
			new(TokenType.Comma)
		};
		parser.TryConsumeOptions(options, optionSeparator);

		parser.ConsumeToken(TokenType.RightParentheses);
	}
}

