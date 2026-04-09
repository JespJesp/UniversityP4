using Lexing.Tokens;

namespace Lexing.Lexers;

public static class StringLexer
{
	public static void Lex()
	{
		string str = "";
		int startLine = Lexer.Cursor.Line;
		int startColumn = Lexer.Cursor.Column;
		Lexer.Cursor.MoveToNextColumn(); // Skip opening quote

		bool isClosingQuote() => Lexer.CursorChar == '"';

		while (!isClosingQuote())
		{
			str += Lexer.CursorChar;
			Lexer.Cursor.MoveToNextColumn();

			if (!Lexer.IsNotEndOfFile || Lexer.CursorChar == '\n')
			{
				Lexer.AddError(new LexicalError(startLine, startColumn, "String is missing closing quote '\"'"));
				return;
			}
		}

		Lexer.Cursor.MoveToNextColumn(); // Skip closing quote
		Lexer.Tokens.Add(new Token(TokenType.String, str, Lexer.Cursor.Line, startColumn));
	}
}