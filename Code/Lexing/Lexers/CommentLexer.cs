namespace Lexing.Lexers;

public static class CommentLexer
{
	public static void Lex()
	{
		int startLine = Lexer.Cursor.Line;
		int startColumn = Lexer.Cursor.Column;

		// Skip opening percentage
		Lexer.Cursor.MoveToNextColumn();

		// Skip until closing percentage
		while (Lexer.CursorChar != '%')
		{
			if (Lexer.CursorChar == '\n')
			{
				Lexer.Cursor.MoveToNewLine();
			}
			else
			{
				Lexer.Cursor.MoveToNextColumn();
			}

			if (Lexer.AtEndOfFile)
			{
				throw new LexicalException(startLine, startColumn, "Comment is missing closing percentage '%'");
			}
		}

		// Skip closing percentage
		Lexer.Cursor.MoveToNextColumn();
	}
}