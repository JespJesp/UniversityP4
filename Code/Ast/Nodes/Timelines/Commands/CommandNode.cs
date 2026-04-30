using Ast.Nodes.Floats;
using Ast.Nodes.Melodies;
using Ast.Nodes.Patterns;
using Phases.Annotation;
using Phases.Evaluation;
using Phases.Parsing;
using Phases.Validation;
using Runtime.Objects.Timelines;
using Tokens;

namespace Ast.Nodes.Timelines.Commands;

public class CommandNode : SymbolNode
{
	public TimelineNode TimelineNode;
	public TimelineCommand Command = new();
	public string CommandType = "";
	private FloatExpressionNode _commandBeat = new();
	private List<string> _commandTargetIds = new();

	public CommandNode(TimelineNode timelineNode)
	{
		this.TimelineNode = timelineNode;
	}

	public override void CascadeParse(Parser parser)
	{
		// Identifier and command, or just command
		parser.ConsumeToken(TokenType.Identifier, out string firstIdentifierValue);
		if (Enum.TryParse(firstIdentifierValue, ignoreCase: true, out TimelineCommandType result))
		{
			Id = "";
			CommandType = firstIdentifierValue.ToLowerInvariant();
		}
		else
		{
			Id = firstIdentifierValue;
			parser.ConsumeToken(TokenType.Identifier, out string typeValue);
			CommandType = typeValue.ToLowerInvariant();
		}

		// Optional beat
		_commandBeat = parser.ParseChild(this, new FloatExpressionNode(isOptional: true));

		// Optional command modifiers
		if (parser.TryConsumeToken(TokenType.LeftParentheses))
		{
			parser.ParseChild(this, new ModifiersNode(this));
		}

		// Command targets
		while (parser.TryConsumeIndent(2))
		{
			if (parser.TryConsumeToken(TokenType.Float, out string lengthPart)) // Check for patterns and melody IDs
			{
				parser.ConsumeToken(TokenType.Identifier, out string namePart);
				_commandTargetIds.Add(lengthPart + namePart);
			}
			else // Check for other IDs, e.g. "EVERYTHING"
			{
				parser.ConsumeToken(TokenType.Identifier, out string identifierValue);
				_commandTargetIds.Add(identifierValue);
			}
		}
	}

	public override void UpsertSymbol(Annotator annotator)
	{
		// Only add start commands with an identifier to the symbol table
		if (Enum.Parse<TimelineCommandType>(CommandType, ignoreCase: true) == TimelineCommandType.Start && Id != "")
		{
			SymbolTable.Upsert(this, Id);
		}
	}

	public override void AfterSymbolUpsert(Annotator annotator)
	{
		// Add start commands with an identifier to the symbol table
		if (Enum.Parse<TimelineCommandType>(CommandType, ignoreCase: true) == TimelineCommandType.Start && CommandId != "")
		{
			SymbolTable.Upsert(this, CommandId);
		}

		foreach (string targetId in _commandTargetIds)
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
		if (_commandBeat.Value < 0)
		{
			errors.Add($"Timeline beat '{_commandBeat.Value}' cannot be negative");
		}
		if (!Enum.TryParse(CommandType, ignoreCase: true, out TimelineCommandType result))
		{
			errors.Add($"Command type '{CommandType}' is undefined");
		}
		if (CommandType == TimelineCommandType.Start.ToString())
		{
			if (_commandTargetIds.Count == 0)
			{
				errors.Add("Start commands must specify at least one target melody or pattern");
			}
		}
		else if (CommandType == TimelineCommandType.Stop.ToString())
		{
			if (!_commandBeat.HasValue)
			{
				errors.Add("Stop commands must specify a beat value");
			}
			if (_commandTargetIds.Count == 0 && string.IsNullOrWhiteSpace(Id))
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
		if (_commandBeat.HasValue)
		{
			Command.Beat = _commandBeat.Value;
		}
		foreach (string targetId in _commandTargetIds)
		{
			Command.TargetIds.Add(targetId);
		}

		Timeline timeline = TimelineNode.Timeline;
		timeline.Commands.Add(Command);
	}
}

