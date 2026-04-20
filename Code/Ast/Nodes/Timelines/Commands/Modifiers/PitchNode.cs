using System.Globalization;
using Phases.Evaluation;
using Phases.Parsing;
using Phases.Validation;
using Runtime.Objects.Timeline;
using Tokens;

namespace Ast.Nodes.Timelines.Commands.Modifiers;

public class PitchNode : Node
{
	public ModifiersNode ModifiersNode;
	private float? _pitchShiftHalfsteps;

	public PitchNode(ModifiersNode modifiersNode)
	{
		this.ModifiersNode = modifiersNode;
	}

	public override void CascadeParse(Parser parser)
	{
		// TODO: This could use a float expression node instead
		parser.ConsumeToken(TokenType.Float, out string pitchShiftHalfstepsValue);
		_pitchShiftHalfsteps = float.Parse(pitchShiftHalfstepsValue, CultureInfo.InvariantCulture);
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
		if (_pitchShiftHalfsteps is not null)
		{
			TimelineCommand command = ModifiersNode.CommandNode.Command;
			command.PitchShiftHalfsteps = _pitchShiftHalfsteps.Value;
		}
	}
}

