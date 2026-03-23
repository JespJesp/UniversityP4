using LexicalAnalysis;
using AST;

namespace SyntaxAnalysis.Parsers;

public static class TimelineParser
{
	public static void Parse(SyntaxAnalyzer a)
	{
		a.ConsumeToken(TokenType.TimelineKeyword);

		ParseTimelineContent(a, a.OutputSong.TheTimeline);
	}

	private static void ParseTimelineContent(SyntaxAnalyzer a, Timeline timeline)
	{
		while (!a.HasConsumedAllTokens() && a.TryConsumeNewLineAndTabs(1))
		{
			string firstIdentifier = a.CursorToken().Value.ToLower();
			
			if (firstIdentifier == "settings")
			{
				ParseSettings(a, timeline.Settings);
			}
			else if (firstIdentifier == "start" || firstIdentifier == "stop")
			{
				// Command without identifier
				string commandType = firstIdentifier;
				a.ConsumeToken(TokenType.Identifier);
				
				switch (commandType)
				{
					case "start": ParseStartCommandWithId(a, timeline, ""); break;
					case "stop": ParseStopCommandWithId(a, timeline, ""); break;
				}
			}
			else
			{
				// Identifier followed by command type
				string commandId = firstIdentifier;
				a.ConsumeToken(TokenType.Identifier);
				
				string commandType = a.CursorToken().Value.ToLower();
				switch (commandType)
				{
					case "start": ParseStartCommandWithId(a, timeline, commandId); break;
					case "stop": ParseStopCommandWithId(a, timeline, commandId); break;
					default: throw new Exception($"Unexpected timeline command type: {commandType}");
				}
			}
		}
	}

	private static void ParseSettings(SyntaxAnalyzer a, TimelineSettings settings)
	{
		a.ConsumeToken(TokenType.Identifier);

		while (!a.HasConsumedAllTokens() && a.TryConsumeNewLineAndTabs(2))
		{
			string settingName = a.CursorToken().Value.ToLower();
			a.ConsumeToken(TokenType.Identifier);

			switch (settingName)
			{
				case "timesignature":
					string numerator = "";
					string denominator = "";

					a.ConsumeToken(TokenType.Integer, () =>
					{
						numerator = a.CursorToken().Value;
					});
					a.ConsumeToken(TokenType.ForwardSlash);
					a.ConsumeToken(TokenType.Integer, () =>
					{
						denominator = a.CursorToken().Value;
					});

					settings.TimeSignature = $"{numerator}/{denominator}";
					break;
				case "bpm":
					a.ConsumeToken(TokenType.Integer, () =>
					{
						settings.Bpm = int.Parse(a.CursorToken().Value);
					});
					break;
				default:
					throw new Exception($"Unknown timeline setting: {settingName}");
			}
		}
	}

	private static void ParseStartCommandWithId(SyntaxAnalyzer a, Timeline timeline, string commandId)
	{
		a.ConsumeToken(TokenType.Identifier);
		
		var command = new TimelineCommand { Type = TimelineCommandType.Start, Id = commandId };
		
		if (a.CursorToken().Type == TokenType.Integer)
		{
			a.ConsumeToken(TokenType.Integer, () =>
			{
				command.Beat = int.Parse(a.CursorToken().Value);
			});
		}

		ParsePatternList(a, command);
		timeline.Commands.Add(command);
	}

	private static void ParseStopCommandWithId(SyntaxAnalyzer a, Timeline timeline, string commandId)
	{
		a.ConsumeToken(TokenType.Identifier);
		
		var command = new TimelineCommand { Type = TimelineCommandType.Stop, Id = commandId };
		
		a.ConsumeToken(TokenType.Integer, () =>
		{
			command.Beat = int.Parse(a.CursorToken().Value);
		});

		ParsePatternList(a, command);
		timeline.Commands.Add(command);
	}

	private static void ParsePatternList(SyntaxAnalyzer a, TimelineCommand command)
	{
		while (!a.HasConsumedAllTokens() && a.TryConsumeNewLineAndTabs(2))
		{
			if (a.CursorToken().Type == TokenType.Integer)
			{
				a.ConsumeToken(TokenType.Integer);
				a.ConsumeToken(TokenType.Identifier, () =>
				{
					command.PatternNames.Add(a.CursorToken().Value);
				});
				continue;
			}

			a.ConsumeToken(TokenType.Identifier, () =>
			{
				command.PatternNames.Add(a.CursorToken().Value);
			});
		}
	}
}