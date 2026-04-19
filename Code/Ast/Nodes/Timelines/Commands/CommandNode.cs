using System.Globalization;
using Ast.Nodes.Melodies;
using Ast.Nodes.Patterns;
using Phases.Annotation;
using Phases.Evaluation;
using Phases.Parsing;
using Phases.Validation;
using Runtime.Objects.Timeline;
using Tokens;

namespace Ast.Nodes.Timelines.Commands;

public class CommandNode : SymbolNode
{
	public TimelineNode TimelineNode;
	public TimelineCommand Command = new();
	public string FirstIdentifier = "";
	public string CommandType = "";
	public float? CommandBeat;
	public List<string> CommandTargetIds = new();

	public CommandNode(TimelineNode timelineNode, string firstIdentifier)
	{
		this.TimelineNode = timelineNode;
		this.FirstIdentifier = firstIdentifier;
	}

	public override void CascadeParse(Parser parser)
	{
		// Identifier and command, or just command
		if (Enum.TryParse(FirstIdentifier, ignoreCase: true, out TimelineCommandType result))
		{
			Id = "";
			CommandType = FirstIdentifier.ToLowerInvariant();
		}
		else
		{
			Id = FirstIdentifier;
			parser.ConsumeToken(TokenType.Identifier, (value) => CommandType = value.ToLowerInvariant());
		}

		// Optional beat
		// TODO: This could use a float expression node instead
		parser.TryConsumeToken(TokenType.Float, value => CommandBeat = float.Parse(value, CultureInfo.InvariantCulture)); 

		// Optional command modifiers
		if (parser.TryConsumeToken(TokenType.LeftParentheses))
		{
			parser.ParseChild(this, new ModifiersNode(this));
		}

		// Command targets
		while (parser.TryConsumeIndent(2))
		{
			string lengthPart = "";
			parser.ConsumeToken(TokenType.Float, value => lengthPart = value);
			parser.ConsumeToken(TokenType.Identifier, value => CommandTargetIds.Add(lengthPart + value));
		}
	}

	public override void AdditionalAnnotation(Annotator annotator)
	{
		foreach(string targetId in CommandTargetIds)
		{
			if (targetId != "EVERYTHING"
				&& !SymbolTable.Contains<PatternNode>(targetId)
				&& !SymbolTable.Contains<MelodyNode>(targetId)
				&& !SymbolTable.Contains<CommandNode>(targetId))
			{
				throw new Exception($"Timeline command type: '{CommandType}'. The pattern, melody, or command reference '{targetId}' is not declared");
			}
		}
	}

	public override void Validate(Validator validator)
	{
		// ADD error for unexpected timeline command
		List<string> errors = new();
		if (CommandBeat < 0)
		{
			errors.Add($"Timeline beat '{CommandBeat}' cannot be negative");
		}
		if (!Enum.TryParse(CommandType, ignoreCase: true, out TimelineCommandType result))
		{
			errors.Add($"Command type '{CommandType}' is undefined");
		}
		if (CommandType == TimelineCommandType.Start.ToString())
		{
			if (CommandTargetIds.Count == 0)
			{
				errors.Add("Start commands must specify at least one target melody or pattern");
			}
		}
		else if (CommandType == TimelineCommandType.Stop.ToString())
		{
			if (!CommandBeat.HasValue)
			{
				errors.Add("Stop commands must specify a beat value");
			}
			if (CommandTargetIds.Count == 0 && string.IsNullOrWhiteSpace(Id))
			{
				errors.Add("Stop commands must specify targets, EVERYTHING, or a command ID");
			}
		}
		if (errors.Count != 0)
		{
			throw new Exception($"Timeline commands." + string.Join(" ", errors));
		}
	}

	public override void Evaluate(Evaluator evaluator)
	{
		Command.Id = Id;
		Command.Type = Enum.Parse<TimelineCommandType>(CommandType, ignoreCase: true);
		if (CommandBeat is not null)
		{
			Command.Beat = CommandBeat.Value;
		}
		foreach (string targetId in CommandTargetIds)
		{
			Command.TargetIds.Add(targetId);
		}

		Timeline timeline = TimelineNode.Timeline;
		timeline.Commands.Add(Command);
	}
}

