using LexicalAnalysis;
using AST;

namespace SyntaxAnalysis.Parsers;

public static class PatternParser
{
	public static void Parse(SyntaxAnalyzer a, Song song)
	{
		Pattern pattern = new(song);
		a.OutputSong.Patterns.Add(pattern);

		a.ConsumeToken(TokenType.Integer, () =>
		{
			pattern.Length = int.Parse(a.CursorToken().Value);
		});

		a.ConsumeToken(TokenType.Identifier, () =>
		{
			pattern.Id = a.CursorToken().Value;
		});

		ParseLeaves(a, pattern);
	}

	private static void ParseLeaves(SyntaxAnalyzer a, Pattern pattern)
	{
		while (!a.HasConsumedAllTokens() && a.TryConsumeIndents(1))
		{
			switch (a.CursorToken().Type)
			{
				case TokenType.NotesKeyword: PatternNotesParser.Parse(a, pattern); break;
				case TokenType.SamplesKeyword: PatternSamplesParser.Parse(a, pattern); break;
				default: throw new ArgumentOutOfRangeException();
			}
		}
	}


}