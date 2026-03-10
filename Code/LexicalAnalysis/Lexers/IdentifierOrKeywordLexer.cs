namespace LexicalAnalysis.Lexers;

public static class IdentifierOrKeywordLexer
{
	public static void Lex(LexicalAnalyzer a)
	{
		string identifier = "";
		int startColumn = a.Cursor.Column;

		while (a.IsNotEndOfFile() && char.IsLetterOrDigit(a.CursorChar()) || a.CursorChar() == '_')
		{
			identifier += a.CursorChar();
			a.Cursor.MoveToNextColumn();
		}

		TokenType tokenType = identifier switch
		{
			"timeline" => TokenType.TimelineKeyword,
			"samples" => TokenType.SamplesKeyword,
			"notes" => TokenType.NotesKeyword,
			_ => TokenType.Identifier // The underscore notation encompasses all other strings
		};

		a.Tokens.Add(new Token(tokenType, identifier, a.Cursor.Line, startColumn));
	}
}