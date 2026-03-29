using LexicalAnalysis;
using AbstractSyntax;

namespace SyntaxAnalysis.Parsers;

public static class PatternParser
{
	public static void Parse(SyntaxAnalyzer a)
	{
		Pattern pattern = new();

		a.ConsumeToken(TokenType.PatternKeyword);

		a.ConsumeToken(TokenType.Integer, () =>
		{
			pattern.LengthInBeats = float.Parse(a.CursorToken().Value);
		});

		a.ConsumeToken(TokenType.Identifier, () =>
		{
			pattern.Id = pattern.LengthInBeats + a.CursorToken().Value;
		});

		RuntimeEnvironment.Patterns.Add(pattern.Id, pattern);

		ParseLeaves(a, pattern);
	}

	private static void ParseLeaves(SyntaxAnalyzer a, Pattern pattern)
	{
		while (!a.HasConsumedAllTokens() && a.TryConsumeIndents(1))
		{
			string length = "";

			a.ConsumeToken(TokenType.Integer, () =>
			{
				length = a.CursorToken().Value;
			});

			a.ConsumeToken(TokenType.Identifier, () =>
			{
				pattern.PatternAndMelodyIds.Add(length + a.CursorToken().Value);
			});
		}
	}
}