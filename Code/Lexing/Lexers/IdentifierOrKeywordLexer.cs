using Lexing.Tokens;

namespace Lexing.Lexers;

public static class IdentifierOrKeywordLexer
{
	public static void Lex()
	{
		string id = "";
		int startColumn = Lexer.Cursor.Column;

		while (Lexer.IsNotEndOfFile && Lexer.CursorChar == '_' || Lexer.CursorChar == '#' || char.IsLetterOrDigit(Lexer.CursorChar))
		{
			id += Lexer.CursorChar;
			Lexer.Cursor.MoveToNextColumn();
		}

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
	}
}