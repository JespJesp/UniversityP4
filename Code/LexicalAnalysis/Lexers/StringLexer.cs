namespace LexicalAnalysis.Lexers;

public static class StringLexer
{
	public static void Lex(LexicalAnalyzer a)
	{
		string str = "";
		int startColumn = a.Cursor.Column;
		a.Cursor.MoveToNextColumn(); // Skip opening quote

		bool isClosingQuote() => a.CursorChar() == '"';

		while (!isClosingQuote())
		{
			str += a.CursorChar();
			a.Cursor.MoveToNextColumn();

			if (!a.IsNotEndOfFile())
			{
				throw new Exception(
					$"String is missing closing quote '\"' at Line:{a.Cursor.Line} Column:{a.Cursor.Column}");
			}
		}

		a.Cursor.MoveToNextColumn(); // Skip closing quote
		a.Tokens.Add(new Token(TokenType.String, str, a.Cursor.Line, startColumn));
	}
}