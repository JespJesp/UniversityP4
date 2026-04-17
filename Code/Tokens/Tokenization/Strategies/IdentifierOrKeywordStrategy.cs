using Phases.Lexing;

namespace Tokens.Tokenization.Strategies;

public class IdentifierOrKeywordStrategy : ITokenizationStrategy
{
	public static bool TryTokenize(Lexer lexer)
	{
		if (lexer.CursorChar != '_' && !char.IsLetter(lexer.CursorChar))
		{
			return false;
		}

		string id = "";
		int startColumn = lexer.Cursor.Column;

		// Chain characters together
		while (!lexer.AtEndOfFile
			&& (lexer.CursorChar == '_' || char.IsLetterOrDigit(lexer.CursorChar)))
		{
			id += lexer.CursorChar;
			lexer.Cursor.MoveToNextColumn();
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

		lexer.Tokens.Add(new Token(tokenType, id, lexer.Cursor.Line, startColumn));

		return true;
	}
}