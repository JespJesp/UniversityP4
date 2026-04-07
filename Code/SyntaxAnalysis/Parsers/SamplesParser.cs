using LexicalAnalysis;
using Runtime;

namespace SyntaxAnalysis.Parsers;

public static class SamplesParser
{
	public static void Parse(SyntaxAnalyzer a)
	{
		a.ConsumeToken(TokenType.SamplesKeyword);

		ParseBranches(a);
	}

	private static void ParseBranches(SyntaxAnalyzer a)
	{
		while (a.TryConsumeIndents(1))
		{
			Sample sample = new();

			a.ConsumeToken(TokenType.Identifier, () =>
			{
				sample.Id = a.CursorToken().Value;
			});

			a.ConsumeToken(TokenType.String, () =>
			{
				sample.FilePath = a.CursorToken().Value;
			});

			a.TryConsumeToken(TokenType.Identifier, () =>
			{
				sample.ReferencePitch = new Pitch(a.CursorToken().Value);
			});

			Runtime.Environment.Samples.Add(sample.Id, sample);
		}
	}
}