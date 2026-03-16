using LexicalAnalysis;
using AST;

namespace SyntaxAnalysis.Parsers;

public static class PatternSamplesParser
{
	public static void Parse(SyntaxAnalyzer a, Pattern pattern)
	{
		a.ConsumeToken(TokenType.SamplesKeyword);

		ParseLeaves(a, pattern);
	}

	private static void ParseLeaves(SyntaxAnalyzer a, Pattern pattern)
	{
		while (!a.HasConsumedAllTokens() && a.TryConsumeIndents(2))
		{
			a.ConsumeToken(TokenType.Identifier, () =>
			{
				pattern.SampleIds.Add(a.CursorToken().Value);
			});
		}
	}
}