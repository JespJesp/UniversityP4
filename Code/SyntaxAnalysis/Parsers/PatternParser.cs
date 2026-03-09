using System.Diagnostics;
using LexicalAnalysis;
using AST;

namespace SyntaxAnalysis.Parsers;

public static class PatternParser
{
	public static void Parse(SyntaxAnalyzer a)
	{
		Pattern pattern = new();
		a.OutputSong.Patterns.Add(pattern);

		a.ConsumeToken(TokenType.Integer, () =>
		{
			pattern.Length = int.Parse(a.CurrentToken().Value);
		});

		a.ConsumeToken(TokenType.Identifier, () =>
		{
			pattern.Name = a.CurrentToken().Value;
		});

		ParseLeaves(a, pattern);
	}

	private static void ParseLeaves(SyntaxAnalyzer a, Pattern pattern)
	{
		while (!a.HasProcessedAllTokens() && a.TryConsumeNewLineAndTabs(1))
		{
			switch (a.CurrentToken().Type)
			{
				case TokenType.NotesKeyword: NotesParser.Parse(a, pattern); break;
				case TokenType.SamplesKeyword: SamplesParser.Parse(a, pattern); break;
				default: throw new Exception();
			}
		}
	}


}