using LexicalAnalysis;
using AST;

namespace SyntaxAnalysis.Parsers;

public static class NotesParser
{
	public static void Parse(SyntaxAnalyzer a, Pattern pattern)
	{
		a.ConsumeToken(TokenType.NotesKeyword);

		ParseLeaves(a, pattern);
	}

	private static void ParseLeaves(SyntaxAnalyzer a, Pattern pattern)
	{
		while (!a.HasConsumedAllTokens() && a.TryConsumeNewLineAndTabs(2))
		{
			Note note = new(pattern);
			

			a.ConsumeToken(TokenType.Integer, () =>
			{
				note.StartTime = int.Parse(a.CursorToken().Value);
			});

			a.ConsumeToken(TokenType.Hyphen);

			a.ConsumeToken(TokenType.Integer, () =>
			{
				note.EndTime = int.Parse(a.CursorToken().Value);
			});

			if (a.CursorToken().Type == TokenType.Identifier && a.CursorToken().Value == "random")
			{
				note.PitchExpression = ExpressionParser.Parse(a);
			}
			else
			{
				a.ConsumeToken(TokenType.Identifier, () =>
				{
					note.Pitch = a.CursorToken().Value;
				});
			}

			// Chords
			while (!a.HasConsumedAllTokens() && a.CursorToken().Type == TokenType.Identifier)
			{
				Note chordNote = new(pattern)
				{
					StartTime = note.StartTime,
					EndTime = note.EndTime,
					Pitch = a.CursorToken().Value
				};

				pattern.Notes.Add(chordNote);

				a.ConsumeToken(TokenType.Identifier);
			}
		}
	}
}