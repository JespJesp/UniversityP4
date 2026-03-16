using LexicalAnalysis;
using AST;

namespace SyntaxAnalysis.Parsers;

public static class PatternNotesParser
{
	public static void Parse(SyntaxAnalyzer a, Pattern pattern)
	{
		a.ConsumeToken(TokenType.NotesKeyword);

		ParseLeaves(a, pattern);
	}

	private static void ParseLeaves(SyntaxAnalyzer a, Pattern pattern)
	{
		while (!a.HasConsumedAllTokens() && a.TryConsumeIndents(2))
		{
			Note note = new(pattern);
			pattern.Notes.Add(note);

			a.ConsumeToken(TokenType.Integer, () =>
			{
				note.StartTime = int.Parse(a.CursorToken().Value);
			});

			a.ConsumeToken(TokenType.Hyphen);

			a.ConsumeToken(TokenType.Integer, () =>
			{
				note.EndTime = int.Parse(a.CursorToken().Value);
			});

			a.ConsumeToken(TokenType.Identifier, () =>
			{
				note.Pitch = new Pitch(a.CursorToken().Value);
			});
		}
	}
}