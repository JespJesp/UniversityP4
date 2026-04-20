using Ast.Nodes.Floats;
using Phases.Evaluation;
using Phases.Parsing;
using Phases.Validation;
using Runtime.Objects.Timelines;

namespace Ast.Nodes.Timelines.Commands.Modifiers;

public class GainNode : Node
{
	public ModifiersNode ModifiersNode;
	private FloatExpressionNode _gainMultiplier = new();

	public GainNode(ModifiersNode modifiersNode)
	{
		this.ModifiersNode = modifiersNode;
	}

	public override void CascadeParse(Parser parser)
	{
		_gainMultiplier = parser.ParseChild(this, new FloatExpressionNode());
	}

	public override void Validate(Validator validator)
	{
		CommandNode commandNode = ModifiersNode.CommandNode;

		if (commandNode.CommandType != TimelineCommandType.Stop.ToString())
		{
			throw new Exception($"Timeline command: '{commandNode.CommandType}'. Command cannot use gain modifiers");
		}

		if (_gainMultiplier.Value < 0)
		{
			throw new Exception($"Timeline command: '{commandNode.CommandType}'. Gain '{_gainMultiplier.Value}' cannot be negative");
		}
	}

	public override void Evaluate(Evaluator evaluator)
	{
		TimelineCommand command = ModifiersNode.CommandNode.Command;
		command.GainMultiplier = _gainMultiplier.Value;
	}
}

