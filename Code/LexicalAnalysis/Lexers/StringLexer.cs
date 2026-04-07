using LexicalAnalysis.Tokens;

namespace LexicalAnalysis.Lexers;

public static class StringLexer
{
	public static void Lex(LexicalAnalyzer a)
	{
		string str = "";
		int startLine = a.Cursor.Line;
		int startColumn = a.Cursor.Column;
		a.Cursor.MoveToNextColumn(); // Skip opening quote

		bool isClosingQuote() => a.CursorChar() == '"';

		while (!isClosingQuote())
		{
			str += a.CursorChar();
			a.Cursor.MoveToNextColumn();

			if (!a.IsNotEndOfFile() || a.CursorChar() == '\n')
			{
				a.AddError(new LexicalError(startLine, startColumn, "String is missing closing quote '\"'"));
				return;
			}
		}

		a.Cursor.MoveToNextColumn(); // Skip closing quote
		a.Tokens.Add(new Token(TokenType.String, str, a.Cursor.Line, startColumn));
	}
}