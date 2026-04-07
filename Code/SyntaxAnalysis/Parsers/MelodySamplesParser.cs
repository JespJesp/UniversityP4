using LexicalAnalysis;
using Runtime;

namespace SyntaxAnalysis.Parsers;

public static class MelodySamplesParser
{
	public static void Parse(SyntaxAnalyzer a, Melody melody)
	{
		a.ConsumeToken(TokenType.SamplesKeyword);

		ParseBranches(a, melody);
	}

	private static void ParseBranches(SyntaxAnalyzer a, Melody melody)
	{
		while (a.TryConsumeIndents(2))
		{
			a.ConsumeToken(TokenType.Identifier, () =>
			{
				melody.SampleIds.Add(a.CursorToken().Value);
			});
		}
	}
}