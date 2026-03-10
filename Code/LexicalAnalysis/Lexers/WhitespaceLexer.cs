namespace LexicalAnalysis.Lexers;

public static class WhitespaceLexer
{
	public static void Lex(LexicalAnalyzer a)
	{
		if (a.CursorChar() == '\n')
		{
			a.Tokens.Add(new Token(TokenType.NewLine, "\\n", a.Cursor.Line, a.Cursor.Column));
			a.Cursor.MoveoToNewLine();
		}
		else if (a.CursorChar() == '\t')
		{
			a.Tokens.Add(new Token(TokenType.Tab, "\\t", a.Cursor.Line, a.Cursor.Column));
			a.Cursor.MoveToNextColumn();
		}
		else // Ignore whitespace if not a tab or a new line
		{
			a.Cursor.MoveToNextColumn();
		}
	}
}