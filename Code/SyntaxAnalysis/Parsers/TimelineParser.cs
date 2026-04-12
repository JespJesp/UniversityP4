using LexicalAnalysis;
using AbstractSyntax;
using System.Globalization;

namespace SyntaxAnalysis.Parsers;

public static class TimelineParser
{
	public static void Parse(SyntaxAnalyzer analyzer)
	{
		analyzer.ConsumeToken(TokenType.TimelineKeyword);
		RuntimeEnvironment.TheTimeline.Reset();

		while (analyzer.TryConsumeIndents(1))
		{
			string firstIdentifier = analyzer.CursorToken().Value;
			if (string.Equals(firstIdentifier, "settings", StringComparison.OrdinalIgnoreCase))
			{
				analyzer.ConsumeToken(TokenType.Identifier);
				ParseSettings(analyzer, RuntimeEnvironment.TheTimeline.Settings);
				continue;
			}

			ParseCommand(analyzer, RuntimeEnvironment.TheTimeline, firstIdentifier);
		}
	}

	private static void ParseSettings(SyntaxAnalyzer analyzer, TimelineSettings settings)
	{
		while (analyzer.TryConsumeIndents(2))
		{
			string settingName = analyzer.CursorToken().Value.ToLowerInvariant();
			analyzer.ConsumeToken(TokenType.Identifier);

			switch (settingName)
			{
				case "timesignature":
					analyzer.ConsumeToken(TokenType.Integer, () =>
					{
						settings.TimeSignatureNumerator = int.Parse(analyzer.CursorToken().Value, CultureInfo.InvariantCulture);
					});
					analyzer.ConsumeToken(TokenType.ForwardSlash);
					analyzer.ConsumeToken(TokenType.Integer, () =>
					{
						settings.TimeSignatureDenominator = int.Parse(analyzer.CursorToken().Value, CultureInfo.InvariantCulture);
					});
					break;
				case "bpm":
					analyzer.ConsumeToken(TokenType.Integer, () =>
					{
						settings.Bpm = int.Parse(analyzer.CursorToken().Value, CultureInfo.InvariantCulture);
					});
					break;
				default:
					throw new Exception($"Unknown timeline setting: {settingName}");
			}
		}
	}

	private static void ParseCommand(SyntaxAnalyzer analyzer, Timeline timeline, string firstIdentifier)
	{
		string commandId = "";
		string commandType = firstIdentifier.ToLowerInvariant();

		if (commandType != "start" && commandType != "stop")
		{
			commandId = firstIdentifier;
			analyzer.ConsumeToken(TokenType.Identifier);
			commandType = analyzer.CursorToken().Value.ToLowerInvariant();
		}

		analyzer.ConsumeToken(TokenType.Identifier);

		TimelineCommand command = commandType switch
		{
			"start" => new TimelineCommand { Id = commandId, Type = TimelineCommandType.Start },
			"stop" => new TimelineCommand { Id = commandId, Type = TimelineCommandType.Stop },
			_ => throw new Exception($"Unexpected timeline command type: {commandType}")
		};

		ParseOptionalCommandBeat(analyzer, command);
		ParseOptionalCommandModifiers(analyzer, command);
		ParseCommandTargets(analyzer, command);
		timeline.Commands.Add(command);
	}

	private static void ParseOptionalCommandBeat(SyntaxAnalyzer analyzer, TimelineCommand command)
	{
		if (analyzer.CursorToken().Type != TokenType.Integer && analyzer.CursorToken().Type != TokenType.Float)
		{
			return;
		}

		analyzer.ConsumeToken(TokenType.Float, () =>
		{
			command.Beat = float.Parse(analyzer.CursorToken().Value, CultureInfo.InvariantCulture);
		});
	}

	private static void ParseOptionalCommandModifiers(SyntaxAnalyzer analyzer, TimelineCommand command)
	{
		if (!analyzer.TryConsumeToken(TokenType.LeftParentheses))
		{
			return;
		}

		while (true)
		{
			if (analyzer.TryConsumeToken(TokenType.GainKeyword))
			{
				analyzer.ConsumeToken(TokenType.Float, () =>
				{
					command.GainMultiplier = float.Parse(analyzer.CursorToken().Value, CultureInfo.InvariantCulture);
				});
			}
			else if (analyzer.CursorToken().Type == TokenType.Identifier && string.Equals(analyzer.CursorToken().Value, "pitch", StringComparison.OrdinalIgnoreCase))
			{
				analyzer.ConsumeToken(TokenType.Identifier);
				analyzer.ConsumeToken(TokenType.Float, () =>
				{
					command.PitchShiftHalfsteps = float.Parse(analyzer.CursorToken().Value, CultureInfo.InvariantCulture);
				});
			}
			else
			{
				throw new Exception("Expected timeline modifier 'gain' or 'pitch'");
			}

			if (!analyzer.TryConsumeToken(TokenType.Comma))
			{
				break;
			}
		}

		analyzer.ConsumeToken(TokenType.RightParentheses);
	}

	private static void ParseCommandTargets(SyntaxAnalyzer analyzer, TimelineCommand command)
	{
		while (analyzer.TryConsumeIndents(2))
		{
			if (analyzer.CursorToken().Type == TokenType.Integer)
			{
				string lengthPart = "";
				analyzer.ConsumeToken(TokenType.Integer, () =>
				{
					lengthPart = analyzer.CursorToken().Value;
				});
				analyzer.ConsumeToken(TokenType.Identifier, () =>
				{
					command.TargetIds.Add(lengthPart + analyzer.CursorToken().Value);
				});
				continue;
			}

			analyzer.ConsumeToken(TokenType.Identifier, () =>
			{
				command.TargetIds.Add(analyzer.CursorToken().Value);
			});
		}
	}
}