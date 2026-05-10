using Ast.Nodes.Timelines.Commands;
using Phases.Parsing;
using Phases.Validation;
using Runtime.Objects.Timelines;
using Tokens;

namespace Ast.Nodes.Timelines;

public class TimelineNode : SymbolNode
{
	public static int InstanceCount = 0;
	public static TimelineNode Instance = new();

	public Timeline Timeline = new();

	public override void CascadeParse(Parser parser)
	{
		// Singleton-esque logic
		Instance = this;
		InstanceCount++;

		while (parser.TryConsumeNewlineIndent(1))
		{
			if (parser.TryConsumeToken(TokenType.Identifier, "settings"))
			{
				parser.ParseChild(this, new SettingsNode(this));
			}
			else if (parser.CursorToken.Type == TokenType.Identifier)
			{
				parser.ParseChild(this, new CommandNode(this));
			}
			else
			{
				throw new Exception($"Unexpected timeline instruction");
			}
		}
	}

	public override void Validate(Validator validator)
	{
		if (InstanceCount > 1)
		{
			throw new Exception("'timeline' keyword appears multiple times");
		}
	}
}
