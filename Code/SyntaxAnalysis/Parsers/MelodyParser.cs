using LexicalAnalysis;
using AbstractSyntax;

namespace SyntaxAnalysis.Parsers;

public static class MelodyParser
{
	public static void Parse(SyntaxAnalyzer a)
	{
		Melody melody = new();

		a.ConsumeToken(TokenType.MelodyKeyword);

		a.ConsumeToken(TokenType.Integer, () =>
		{
			melody.Length = int.Parse(a.CursorToken().Value);
		});

		a.ConsumeToken(TokenType.Identifier, () =>
		{
			melody.Id = melody.Length + a.CursorToken().Value;
		});

		AST.Melodies.Add(melody.Id, melody);

		ParseLeaves(a, melody);
	}

	private static void ParseLeaves(SyntaxAnalyzer a, Melody melody)
	{
		while (!a.HasConsumedAllTokens() && a.TryConsumeIndents(1))
		{
			switch (a.CursorToken().Type)
			{
				case TokenType.NotesKeyword: MelodyNotesParser.Parse(a, melody); break;
				case TokenType.SamplesKeyword: MelodySamplesParser.Parse(a, melody); break;
				default: throw new ArgumentOutOfRangeException();
			}
		}
	}


}