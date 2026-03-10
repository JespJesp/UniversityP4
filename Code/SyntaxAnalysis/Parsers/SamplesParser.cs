using LexicalAnalysis;
using AST;

namespace SyntaxAnalysis.Parsers;

public static class SamplesParser
{
	public static void Parse(SyntaxAnalyzer a, Pattern pattern)
	{
		a.ConsumeToken(TokenType.SamplesKeyword);

		ParseLeaves(a, pattern);
	}

	private static void ParseLeaves(SyntaxAnalyzer a, Pattern pattern)
	{
		while (!a.HasConsumedAllTokens() && a.TryConsumeNewLineAndTabs(2))
		{
			Sample sample = new();
			pattern.Samples.Add(sample);

			a.ConsumeToken(TokenType.String, () =>
			{
				sample.FileName = a.CursorToken().Value;
			});
		}
	}
}