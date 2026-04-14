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

		pattern.Length = ParseLengthExpression(a);

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

	private static int ParseLengthExpression(SyntaxAnalyzer a)
	{
		int value = ParsePrimary(a);

		while (a.CurrentToken().Type == TokenType.Multiply)
		{
			a.ConsumeToken(TokenType.Multiply);

			int right = ParsePrimary(a);

			value *= right;
		}

		return value;
	}

	private static int ParsePrimary(SyntaxAnalyzer a)
	{
		int number = 0;

		a.ConsumeToken(TokenType.Integer, () =>
		{
			number = int.Parse(a.CurrentToken().Value);
		});

		// Check for 'm'
		if (a.CurrentToken().Type == TokenType.MeasureSuffix)
		{
			a.ConsumeToken(TokenType.MeasureSuffix);

			number *= BeatsPerMeasure;
		}

		return number;
	}

	private const int BeatsPerMeasure = 4;
}