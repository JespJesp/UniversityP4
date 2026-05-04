using Ast.Tables;
using Lexing.Tokens;
using Runtime;
using Runtime.Objects;
using System.Globalization;

namespace Ast.Nodes.Timelines;

public class TimelineNode(Node parent, bool createsNestedScope = false) : VariableNode(parent, createsNestedScope)
{
	private readonly TimelineSettings _settings = new();
	private readonly List<TimelineCommand> _commands = new();
	public Timeline Timeline0 = new();

	protected override void Parse()
	{
		Parser.ConsumeToken(TokenType.TimelineKeyword);
		Id = "timeline";

		while (parser.TryConsumeNewlineIndent(1))
		{
			string firstIdentifier = Parser.CurrentToken.Value;
			if (string.Equals(firstIdentifier, "settings", StringComparison.OrdinalIgnoreCase))
			{
				Parser.ConsumeToken(TokenType.Identifier);
				ParseSettings();
				continue;
			}

			ParseCommand(firstIdentifier);
		}
	}

	protected override void AdditionalValidation(NodeTable ancestors, SemanticSymbolTable symbols)
	{
		if (_settings.Bpm <= 0)
		{
			Validator.AddError(this, $"BPM must be positive, got: {_settings.Bpm}");
		}

		if (_settings.TimeSignatureNumerator <= 0 || _settings.TimeSignatureDenominator <= 0)
		{
			Validator.AddError(this, $"Time signature values must be positive, got: {_settings.TimeSignatureNumerator}/{_settings.TimeSignatureDenominator}");
		}

		foreach (TimelineCommand command in _commands)
		{
			ValidateCommand(command);
		}
	}

	protected override void AdditionalEvaluation(NodeTable ancestors, RuntimeVariableTable variables)
	{
		Timeline0.BeatsPerMinute = _settings.Bpm;
		Timeline0.BeatsPerBar = _settings.TimeSignatureNumerator;
		Timeline0.BeatNoteValue = _settings.TimeSignatureDenominator;
		Timeline0.Commands = _commands
			.Select(command => new TimelineCommand
			{
				Id = command.Id,
				Type = command.Type,
				Beat = command.Beat,
				TargetIds = new List<string>(command.TargetIds),
				GainMultiplier = command.GainMultiplier,
				PitchShiftHalfsteps = command.PitchShiftHalfsteps
			})
			.ToList();
	}

	protected override RuntimeObject GetRuntimeObject()
	{
		return Timeline0;
	}

	private void ParseSettings()
	{
		while (Parser.TryConsumeIndent(2))
		{
			string settingName = Parser.CurrentToken.Value.ToLowerInvariant();
			Parser.ConsumeToken(TokenType.Identifier);

			switch (settingName)
			{
				case "timesignature":
					Parser.ConsumeToken(TokenType.Integer, value => _settings.TimeSignatureNumerator = int.Parse(value, CultureInfo.InvariantCulture));
					Parser.ConsumeToken(TokenType.ForwardSlash);
					Parser.ConsumeToken(TokenType.Integer, value => _settings.TimeSignatureDenominator = int.Parse(value, CultureInfo.InvariantCulture));
					break;
				case "bpm":
					Parser.ConsumeToken(TokenType.Integer, value => _settings.Bpm = int.Parse(value, CultureInfo.InvariantCulture));
					break;
				default:
					throw new Exception($"Unknown timeline setting: {settingName}");
			}
		}
	}

	private void ParseCommand(string firstIdentifier)
	{
		string commandId = "";
		string commandType = firstIdentifier.ToLowerInvariant();

		if (commandType != "start" && commandType != "stop")
		{
			commandId = firstIdentifier;
			Parser.ConsumeToken(TokenType.Identifier);
			commandType = Parser.CurrentToken.Value.ToLowerInvariant();
		}

		Parser.ConsumeToken(TokenType.Identifier);

		TimelineCommand command = commandType switch
		{
			"start" => new TimelineCommand { Id = commandId, Type = TimelineCommandType.Start },
			"stop" => new TimelineCommand { Id = commandId, Type = TimelineCommandType.Stop },
			_ => throw new Exception($"Unexpected timeline command type: {commandType}")
		};

		ParseOptionalCommandBeat(command);
		ParseOptionalCommandModifiers(command);
		ParseCommandTargets(command);
		_commands.Add(command);
	}

	private static void ParseOptionalCommandBeat(TimelineCommand command)
	{
		if (Parser.CurrentToken.Type != TokenType.Integer && Parser.CurrentToken.Type != TokenType.Float)
		{
			return;
		}

		Parser.ConsumeToken(TokenType.Float, value => command.Beat = float.Parse(value, CultureInfo.InvariantCulture));
	}

	private static void ParseOptionalCommandModifiers(TimelineCommand command)
	{
		if (!Parser.TryConsumeToken(TokenType.LeftParentheses))
		{
			return;
		}

		bool hasGain = false;
		bool hasPitch = false;

		while (true)
		{
			if (Parser.TryConsumeToken(TokenType.GainKeyword))
			{
				if (hasGain)
				{
					throw new Exception("Duplicate timeline modifier 'gain'");
				}
				hasGain = true;
				Parser.ConsumeToken(TokenType.Float, value => command.GainMultiplier = float.Parse(value, CultureInfo.InvariantCulture));
			}
			else if (Parser.CurrentToken.Type == TokenType.Identifier && string.Equals(Parser.CurrentToken.Value, "pitch", StringComparison.OrdinalIgnoreCase))
			{
				if (hasPitch)
				{
					throw new Exception("Duplicate timeline modifier 'pitch'");
				}
				hasPitch = true;
				Parser.ConsumeToken(TokenType.Identifier);
				Parser.ConsumeToken(TokenType.Float, value => command.PitchShiftHalfsteps = float.Parse(value, CultureInfo.InvariantCulture));
			}
			else
			{
				throw new Exception("Expected timeline modifier 'gain' or 'pitch'");
			}

			if (!Parser.TryConsumeToken(TokenType.Comma))
			{
				break;
			}
		}

		Parser.ConsumeToken(TokenType.RightParentheses);
	}

	private static void ParseCommandTargets(TimelineCommand command)
	{
		while (Parser.TryConsumeIndent(2))
		{
			if (Parser.CurrentToken.Type == TokenType.Integer)
			{
				string lengthPart = "";
				Parser.ConsumeToken(TokenType.Integer, value => lengthPart = value);
				Parser.ConsumeToken(TokenType.Identifier, value => command.TargetIds.Add(lengthPart + value));
				continue;
			}

			Parser.ConsumeToken(TokenType.Identifier, value => command.TargetIds.Add(value));
		}
	}

	private void ValidateCommand(TimelineCommand command)
	{
		if (command.Beat.HasValue && command.Beat.Value < 0)
		{
			Validator.AddError(this, $"Timeline beat cannot be negative, got: {command.Beat}");
		}

		if (command.Type == TimelineCommandType.Start && command.TargetIds.Count == 0)
		{
			Validator.AddError(this, "Start commands must specify at least one target melody or pattern");
		}

		if (command.Type == TimelineCommandType.Start && command.GainMultiplier < 0)
		{
			Validator.AddError(this, $"Timeline gain cannot be negative, got: {command.GainMultiplier}");
		}

		if (command.Type == TimelineCommandType.Stop && !command.Beat.HasValue)
		{
			Validator.AddError(this, "Stop commands must specify a beat value");
		}

		if (command.Type == TimelineCommandType.Stop && (command.GainMultiplier != 1.0f || command.PitchShiftHalfsteps != 0.0f))
		{
			Validator.AddError(this, "Stop commands cannot use gain or pitch modifiers");
		}

		if (command.Type == TimelineCommandType.Stop && command.TargetIds.Count == 0 && string.IsNullOrWhiteSpace(command.Id))
		{
			Validator.AddError(this, "Stop commands must specify targets, EVERYTHING, or a command ID");
		}
	}
}

internal class TimelineSettings
{
	public int Bpm = 120;
	public int TimeSignatureNumerator = 4;
	public int TimeSignatureDenominator = 4;
}

