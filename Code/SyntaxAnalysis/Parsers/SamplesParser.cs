using LexicalAnalysis;
using AbstractSyntax;

namespace SyntaxAnalysis.Parsers;

public static class SamplesParser
{
	public static void Parse(SyntaxAnalyzer a)
	{
		a.ConsumeToken(TokenType.SamplesKeyword);

		ParseLeaves(a);
	}

	private static void ParseLeaves(SyntaxAnalyzer a)
	{
		while (!a.HasConsumedAllTokens() && a.TryConsumeIndents(1))
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

			AST.Samples.Add(sample.Id, sample);
		}
	}
}