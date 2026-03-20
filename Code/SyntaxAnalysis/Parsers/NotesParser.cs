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
			pattern.Notes.Add(note);

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
			if (a.CurrentToken().Type == TokenType.GainKeyword)
{
    		a.ConsumeToken(TokenType.GainKeyword);
   		a.ConsumeToken(TokenType.Integer, () =>
    		{
        		note.Volume = int.Parse(a.CurrentToken().Value) / 100f;
    });
}
		}
	}
}