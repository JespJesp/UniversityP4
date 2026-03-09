using System.Diagnostics;
using LexicalAnalysis;
using AST;

namespace SyntaxAnalysis.Parsers;

public static class PatternParser
{
	public static void Parse(SyntaxAnalyzer a)
	{
		Pattern newPattern = new();
		a.NewSong.Patterns.Add(newPattern);

		a.ProcessToken(TokenType.Integer, () =>
		{
			newPattern.Length = int.Parse(a.CurrentToken().Value);
		});

		a.ProcessToken(TokenType.Identifier, () =>
		{
			newPattern.Name = a.CurrentToken().Value;
		});

		ParseLeaves(a, newPattern);
	}

	private static void ParseLeaves(SyntaxAnalyzer a, Pattern pattern)
	{
		while (!a.HasProcessedAllTokens() && !a.HasNewLineTabs(1))
		{

			switch (a.CurrentToken().Type)
			{
				case TokenType.KeywordNotes: NotesParser.Parse(a, pattern); break;
				case TokenType.KeywordSamples: SamplesParser.Parse(a, pattern); break;
				default: throw new Exception();
			}
		}
	}


}