using LexicalAnalysis;
using AST;

namespace SyntaxAnalysis.Parsers;

public static class SamplesParser
{
	public static void Parse(SyntaxAnalyzer a, Pattern pattern)
	{
		ParseLeaves(a, pattern);
	}

	private static void ParseLeaves(SyntaxAnalyzer a, Pattern pattern)
	{
		while (!a.HasProcessedAllTokens() && !a.HasNewLineTabs(2))
		{
			Sample sample = new();
			pattern.Samples.Add(sample);

			a.ProcessToken(TokenType.String, () =>
			{
				sample.FileName = a.CurrentToken().Value;
			});
		}
	}
}