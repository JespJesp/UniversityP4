using LexicalAnalysis;
using AST;

namespace SyntaxAnalysis.Parsers;

public static class ScaleParser
{
	public static void Parse(SyntaxAnalyzer a)
	{
		Scale scale = new();
		a.OutputSong.Scales.Add(scale);

		// consume 'scale'
		a.ConsumeToken(TokenType.ScaleKeyword);

		// scale name
		a.ConsumeToken(TokenType.Identifier, () =>
		{
			scale.Name = a.CurrentToken().Value;
		});

		ParseNotes(a, scale);
	}

	private static void ParseNotes(SyntaxAnalyzer a, Scale scale)
	{
		// First note
		a.ConsumeToken(TokenType.Identifier, () =>
		{
			scale.Notes.Add(a.CurrentToken().Value);
		});

		// Remaining notes separated by commas
		while (a.CurrentToken().Type == TokenType.Comma)
		{
			a.ConsumeToken(TokenType.Comma);

			a.ConsumeToken(TokenType.Identifier, () =>
			{
				scale.Notes.Add(a.CurrentToken().Value);
			});
		}
	}
}