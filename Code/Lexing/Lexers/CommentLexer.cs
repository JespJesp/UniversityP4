namespace Lexing.Lexers;

public static class CommentLexer
{
	public static void Lex()
	{
		int startLine = Lexer.Cursor.Line;
		int startColumn = Lexer.Cursor.Column;

		Lexer.Cursor.MoveToNextColumn(); // Skip opening percentage
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

			if (!Lexer.IsNotEndOfFile)
			{
				Lexer.AddError(new LexicalError(startLine, startColumn, "Comment is missing closing percentage '%'"));
				return;
			}
		}
		Lexer.Cursor.MoveToNextColumn(); // Skip closing percentage
	}
}