namespace LexicalAnalysis.Lexers;

public static class CommentLexer
{
	public static void Lex(LexicalAnalyzer a)
	{
		int startLine = a.Cursor.Line;
		int startColumn = a.Cursor.Column;

		a.Cursor.MoveToNextColumn(); // Skip opening percentage
		while (a.CursorChar() != '%')
		{
			if (a.CursorChar() == '\n')
			{
				a.Cursor.MoveToNewLine();
			}
			else
			{
				a.Cursor.MoveToNextColumn();
			}

			if (!a.IsNotEndOfFile())
			{
				a.AddError(new LexicalError(startLine, startColumn, "Comment is missing closing percentage '%'"));
				return;
			}
		}
		a.Cursor.MoveToNextColumn(); // Skip closing percentage
	}
}