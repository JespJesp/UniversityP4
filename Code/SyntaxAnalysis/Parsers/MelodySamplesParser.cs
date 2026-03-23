using LexicalAnalysis;
using AbstractSyntax;

namespace SyntaxAnalysis.Parsers;

public static class MelodySamplesParser
{
	public static void Parse(SyntaxAnalyzer a, Melody melody)
	{
		a.ConsumeToken(TokenType.SamplesKeyword);

		ParseLeaves(a, melody);
	}

	private static void ParseLeaves(SyntaxAnalyzer a, Melody melody)
	{
		while (!a.HasConsumedAllTokens() && a.TryConsumeIndents(2))
		{
			a.ConsumeToken(TokenType.Identifier, () =>
			{
				melody.SampleIds.Add(a.CursorToken().Value);
			});
		}
	}
}