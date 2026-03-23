namespace LexicalAnalysis.Lexers;

public static class ForwardSlashLexer
{
	public static void Lex(LexicalAnalyzer a)
	{
		a.Tokens.Add(new Token(TokenType.ForwardSlash, "/", a.Cursor.Line, a.Cursor.Column));
		a.Cursor.MoveToNextColumn();
	}
}