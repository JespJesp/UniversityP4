using LexicalAnalysis;
using AST;

namespace SyntaxAnalysis.Parsers;

public static class NotesParser
{
	public static void Parse(SyntaxAnalyzer a, Pattern pattern)
	{
		a.ProcessToken(TokenType.KeywordNotes);

		ParseLeaves(a, pattern);
	}

	private static void ParseLeaves(SyntaxAnalyzer a, Pattern pattern)
	{
		while (!a.HasProcessedAllTokens() && !a.HasNewLineTabs(2))
		{
			Note note = new(pattern);
			pattern.Notes.Add(note);

			a.ProcessToken(TokenType.Integer, () =>
			{
				note.StartTime = int.Parse(a.CurrentToken().Value);
			});

			a.ProcessToken(TokenType.Hyphen);

			a.ProcessToken(TokenType.Integer, () =>
			{
				note.EndTime = int.Parse(a.CurrentToken().Value);
			});

			a.ProcessToken(TokenType.Identifier, () =>
			{
				note.Pitch = a.CurrentToken().Value;
			});
		}
	}
}