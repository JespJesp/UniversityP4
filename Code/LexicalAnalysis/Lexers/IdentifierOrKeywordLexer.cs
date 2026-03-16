namespace LexicalAnalysis.Lexers;

public static class IdentifierOrKeywordLexer
{
	public static void Lex(LexicalAnalyzer a)
	{
		string id = "";
		int startColumn = a.Cursor.Column;

		while (a.IsNotEndOfFile() && a.CursorChar() == '_' || a.CursorChar() == '#' || char.IsLetterOrDigit(a.CursorChar()))
		{
			id += a.CursorChar();
			a.Cursor.MoveToNextColumn();
		}

		TokenType tokenType = id switch
		{
			"timeline" => TokenType.TimelineKeyword,
			"samples" => TokenType.SamplesKeyword,
			"notes" => TokenType.NotesKeyword,
			_ => TokenType.Identifier // The underscore notation encompasses all other strings
		};

		a.Tokens.Add(new Token(tokenType, id, a.Cursor.Line, startColumn));
	}
}