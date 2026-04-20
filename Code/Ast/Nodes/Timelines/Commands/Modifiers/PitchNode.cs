using Ast.Nodes.Floats;
using Phases.Evaluation;
using Phases.Parsing;
using Phases.Validation;
using Runtime.Objects.Timelines;

namespace Ast.Nodes.Timelines.Commands.Modifiers;

public class PitchNode : Node
{
	public ModifiersNode ModifiersNode;
	private FloatExpressionNode _pitchShiftHalfsteps = new();

	public PitchNode(ModifiersNode modifiersNode)
	{
		this.ModifiersNode = modifiersNode;
	}

	public override void CascadeParse(Parser parser)
	{
		_pitchShiftHalfsteps = parser.ParseChild(this, new FloatExpressionNode());
	}

	public override void Validate(Validator validator)
	{
		CommandNode commandNode = ModifiersNode.CommandNode;

		if (commandNode.CommandType != TimelineCommandType.Stop.ToString())
		{
			throw new Exception($"Timeline command: '{commandNode.CommandType}'. Command cannot use pitch modifiers");
		}
	}

	public override void Evaluate(Evaluator evaluator)
	{
		TimelineCommand command = ModifiersNode.CommandNode.Command;
		command.PitchShiftHalfsteps = _pitchShiftHalfsteps.Value;
	}
}

