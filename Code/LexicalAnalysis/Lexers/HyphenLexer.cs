namespace LexicalAnalysis.Lexers;

public static class HyphenLexer
{
	public static void Lex(LexicalAnalyzer a)
	{
		a.Tokens.Add(new Token(TokenType.Hyphen, "-", a.CursorLine, a.CursorColumn));
		a.AdvanceCursorToNextColumn();
	}
}