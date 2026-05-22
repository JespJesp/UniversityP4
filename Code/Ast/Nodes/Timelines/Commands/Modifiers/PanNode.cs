using Ast.Nodes.Floats;
using Phases.Evaluation;
using Phases.Parsing;
using Phases.Validation;
using Runtime.Objects.Timelines;

namespace Ast.Nodes.Timelines.Commands.Modifiers;

public class PanNode : Node
{
	public ModifiersNode ModifiersNode;
	private FloatExpressionNode _panOffset = new();

	public PanNode(ModifiersNode modifiersNode)
	{
		this.ModifiersNode = modifiersNode;
	}

	public override void CascadeParse(Parser parser)
	{
		_panOffset = parser.ParseChild(this, new FloatExpressionNode());
	}

	public override void Validate(Validator validator)
	{
		CommandNode commandNode = ModifiersNode.CommandNode;

		if (commandNode.CommandType != TimelineCommandType.Start)
		{
			throw new Exception($"Timeline command: '{commandNode.CommandType}'. Command cannot use pan modifiers");
		}

		if (_panOffset.Value < -1.0f || _panOffset.Value > 1.0f)
		{
			throw new Exception($"Timeline command: '{commandNode.CommandType}'. Pan '{_panOffset.Value}' must be between -1 and 1");
		}
	}

	public override void Evaluate(Evaluator evaluator)
	{
		TimelineCommand command = ModifiersNode.CommandNode.Command;
		command.PanOffset = _panOffset.Value;
	}
}
