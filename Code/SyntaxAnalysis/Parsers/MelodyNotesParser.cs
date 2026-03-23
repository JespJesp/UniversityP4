using LexicalAnalysis;
using AbstractSyntax;

namespace SyntaxAnalysis.Parsers;

public static class MelodyNotesParser
{
	public static void Parse(SyntaxAnalyzer a, Melody melody)
	{
		a.ConsumeToken(TokenType.NotesKeyword);

		ParseLeaves(a, melody);
	}

	private static void ParseLeaves(SyntaxAnalyzer a, Melody melody)
	{
		while (!a.HasConsumedAllTokens() && a.TryConsumeIndents(2))
		{
			Note note = new(melody);
			melody.Notes.Add(note);

			a.ConsumeToken(TokenType.Integer, () =>
			{
				note.StartBeat = float.Parse(a.CursorToken().Value);
			});

			a.ConsumeToken(TokenType.Hyphen);

			a.ConsumeToken(TokenType.Integer, () =>
			{
				note.EndBeat = float.Parse(a.CursorToken().Value);
			});

			a.ConsumeToken(TokenType.Identifier, () =>
			{
				note.ThePitch = new Pitch(a.CursorToken().Value);
			});
		}
	}
}