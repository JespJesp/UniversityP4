using System.Globalization;
using Ast.Nodes.Floats;
using Phases.Evaluation;
using Phases.Parsing;
using Phases.Validation;
using Runtime.Objects.Timelines;
using Tokens;

namespace Ast.Nodes.Timelines.Commands.Modifiers;

public class GainNode : Node
{
	public ModifiersNode ModifiersNode;
	private float? _gainMultiplier;

	public GainNode(ModifiersNode modifiersNode)
	{
		this.ModifiersNode = modifiersNode;
	}

	public override void CascadeParse(Parser parser)
	{
		// TODO: This could use a float expression node instead
		parser.ConsumeToken(TokenType.Float, out string gainMultiplierValue);
		_gainMultiplier = float.Parse(gainMultiplierValue, CultureInfo.InvariantCulture);
	}

	public override void Validate(Validator validator)
	{
		CommandNode commandNode = ModifiersNode.CommandNode;

		if (commandNode.CommandType != TimelineCommandType.Stop.ToString())
		{
			throw new Exception($"Timeline command: '{commandNode.CommandType}'. Command cannot use gain modifiers");
		}

		if (_gainMultiplier < 0)
		{
			throw new Exception($"Timeline command: '{commandNode.CommandType}'. Gain '{_gainMultiplier}' cannot be negative");
		}
	}

	public override void Evaluate(Evaluator evaluator)
	{
		if (_gainMultiplier is not null)
		{
			TimelineCommand command = ModifiersNode.CommandNode.Command;
			command.GainMultiplier = _gainMultiplier.Value;
		}
	}
}

