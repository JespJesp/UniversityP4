namespace LexicalAnalysis.Lexers;

public static class WhitespaceLexer
{
	public static void Lex(LexicalAnalyzer a)
	{
		if (a.CursorChar() == '\n')
		{
			a.Tokens.Add(new Token(TokenType.NewLine, "\\n", a.CursorLine, a.CursorColumn));
			a.AdvanceCursorToNewLine();
		}
		else if (a.CursorChar() == '\t')
		{
			a.Tokens.Add(new Token(TokenType.Tab, "\\t", a.CursorLine, a.CursorColumn));
			a.AdvanceCursorToNextColumn();
		}
		else // Ignore whitespace if not a tab or a new line
		{
			a.AdvanceCursorToNextColumn();
		}
	}
}