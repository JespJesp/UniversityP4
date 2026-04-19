using Ast.Nodes.Timelines.Commands;
using Phases.Parsing;
using Phases.Validation;
using Runtime.Objects.Timeline;
using Tokens;

namespace Ast.Nodes.Timelines;

public class TimelineNode : SymbolNode
{
	public static int TimelineNodeInstances = 0;

	public Timeline Timeline = new();

	public TimelineNode()
	{
		TimelineNodeInstances++;
	}

	public override void CascadeParse(Parser parser)
	{
		Id = "timeline"; // TODO: Jesp: The Id should just be nothing/empty, probably.

		while (parser.TryConsumeIndent(1))
		{
			if (parser.TryConsumeToken(TokenType.Identifier, "settings", (value) =>
				{
					parser.ParseChild(this, new SettingsNode(this));
				})
				|| parser.TryConsumeToken(TokenType.Identifier, (value) =>
				{
					parser.ParseChild(this, new CommandNode(this, value));
				}))
			{
				throw new Exception($"Unexpected timeline instruction");
			}
		}
	}

	public override void Validate(Validator validator)
	{
		if (TimelineNodeInstances > 1)
		{
			throw new Exception("'timeline' keyword appears multiple times.");
		}
	}
}
