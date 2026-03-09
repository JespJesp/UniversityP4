using LexicalAnalysis;

namespace SyntaxAnalysis.Parsers;

public static class TimelineParser
{
	public static void Parse(SyntaxAnalyzer a)
	{
		a.ConsumeToken(TokenType.TimelineKeyword);

		// TODO: Not implemented yet
	}
}