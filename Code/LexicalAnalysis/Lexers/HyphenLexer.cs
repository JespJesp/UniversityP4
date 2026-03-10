namespace LexicalAnalysis.Lexers;

public static class HyphenLexer
{
	public static void Lex(LexicalAnalyzer a)
	{
		a.Tokens.Add(new Token(TokenType.Hyphen, "-", a.Cursor.Line, a.Cursor.Column));
		a.Cursor.MoveToNextColumn();
	}
}