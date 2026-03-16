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
		while (!a.HasProcessedAllTokens() && a.TryConsumeNewLineAndTabs(2))
		{
			Note note = new(pattern);
			

			a.ConsumeToken(TokenType.Integer, () =>
			{
				note.StartTime = int.Parse(a.CurrentToken().Value);
			});

			a.ConsumeToken(TokenType.Hyphen);

			a.ConsumeToken(TokenType.Integer, () =>
			{
				note.EndTime = int.Parse(a.CurrentToken().Value);
			});

			a.ConsumeToken(TokenType.Identifier, () =>
			{
				note.Pitch = a.CurrentToken().Value;
			});

			pattern.Notes.Add(note);
			
			// If more pitches follow, create additional notes with the same timing (chord)
			while (!a.HasProcessedAllTokens() && a.CurrentToken().Type == TokenType.Identifier)
			{
				Note chordNote = new(pattern)
				{
					StartTime = note.StartTime,
					EndTime = note.EndTime,
					Pitch = a.CurrentToken().Value
				};

				pattern.Notes.Add(chordNote);

				a.ConsumeToken(TokenType.Identifier);
			}
		}	
	}
}