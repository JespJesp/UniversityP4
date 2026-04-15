using Lexing;

namespace Tokens.TokenizationStrategies;

public class TokenizeIdentifierOrKeyword : ITokenizationStrategy
{
	public static bool TryTokenize()
	{
		if (Lexer.CursorChar != '_' || !char.IsLetter(Lexer.CursorChar))
		{
			return false;
		}

		string id = "";
		int startColumn = Lexer.Cursor.Column;

		// Chain characters together
		while (!Lexer.AtEndOfFile
			&& (Lexer.CursorChar == '_' || char.IsLetterOrDigit(Lexer.CursorChar)))
		{
			id += Lexer.CursorChar;
			Lexer.Cursor.MoveToNextColumn();
		}

		// Check whether it is a keyword
		TokenType tokenType = id switch
		{
			"timeline" => TokenType.TimelineKeyword,
			"pattern" => TokenType.PatternKeyword,
			"melody" => TokenType.MelodyKeyword,
			"sample" => TokenType.SampleKeyword,
			"samples" => TokenType.SamplesKeyword,
			"chords" => TokenType.ChordsKeyword,
			"gain" => TokenType.GainKeyword,
			"pan" => TokenType.PanKeyword,
			"string" => TokenType.StringKeyword,
			"float" => TokenType.FloatKeyword,
			_ => TokenType.Identifier // The underscore notation encompasses all other strings
		};

		Lexer.Tokens.Add(new Token(tokenType, id, Lexer.Cursor.Line, startColumn));

		return true;
	}
}