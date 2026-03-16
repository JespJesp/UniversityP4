using LexicalAnalysis;
using AST;

namespace SyntaxAnalysis.Parsers;

public static class SamplesParser
{
	public static void Parse(SyntaxAnalyzer a, Song song)
	{
		a.ConsumeToken(TokenType.SamplesKeyword);

		ParseLeaves(a, song);
	}

	private static void ParseLeaves(SyntaxAnalyzer a, Song song)
	{
		while (!a.HasConsumedAllTokens() && a.TryConsumeIndents(1))
		{
			Sample sample = new(song);
			song.Samples.Add(sample);

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
		}
	}
}