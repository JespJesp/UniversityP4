using LexicalAnalysis;
using AbstractSyntax;
using System.Globalization;

namespace SyntaxAnalysis.Parsers;

public static class MelodyParser
{
	public static void Parse(SyntaxAnalyzer a)
	{
		Melody melody = new();

		a.ConsumeToken(TokenType.MelodyKeyword);

		a.ConsumeToken(TokenType.Integer, () =>
		{
			melody.LengthInBeats = float.Parse(a.CursorToken().Value, CultureInfo.InvariantCulture);
		});

		a.ConsumeToken(TokenType.Identifier, () =>
		{
			melody.Id = melody.LengthInBeats + a.CursorToken().Value;
		});

		RuntimeEnvironment.Melodies.Add(melody.Id, melody);

		ParseBranches(a, melody);
	}

	private static void ParseBranches(SyntaxAnalyzer a, Melody melody)
	{
		while (a.TryConsumeIndents(1))
		{
			switch (a.CursorToken().Type)
			{
				case TokenType.NotesKeyword: MelodyNotesParser.Parse(a, melody); break;
				case TokenType.SamplesKeyword: MelodySamplesParser.Parse(a, melody); break;
				default: throw new ArgumentOutOfRangeException();
			}
		}
	}


}