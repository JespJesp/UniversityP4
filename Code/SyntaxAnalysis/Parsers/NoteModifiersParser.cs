using LexicalAnalysis;
using AbstractSyntax;
using System.Globalization;

namespace SyntaxAnalysis.Parsers;

public static class NoteModifiersParser
{
	public static void TryParse(SyntaxAnalyzer a, Note note)
	{
		if (a.TryConsumeToken(TokenType.LeftParentheses))
		{
			TryParseModifiers(a, note);

			a.ConsumeToken(TokenType.RightParentheses);
		}
	}

	public static void TryParseModifiers(SyntaxAnalyzer a, Note note)
	{
		Dictionary<TokenType, Action> cases = new();

		cases.Add(TokenType.GainKeyword, () =>
		{
			a.ConsumeToken(TokenType.Float, () =>
			{
				note.Volume = float.Parse(a.CursorToken().Value, CultureInfo.InvariantCulture);
			});
		});

		cases.Add(TokenType.PanKeyword, () =>
		{
			a.ConsumeToken(TokenType.Float, () =>
			{
				note.Pan = float.Parse(a.CursorToken().Value, CultureInfo.InvariantCulture);
			});
		});

		a.ConsumeUniqueOptions(a, cases, TokenType.Comma);
	}
}