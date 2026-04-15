using Lexing.Tokens;

namespace Lexing.Lexers;

public static class StringLexer
{
	public static void Lex()
	{
		string value = "";
		int startLine = Lexer.Cursor.Line;
		int startColumn = Lexer.Cursor.Column;

		// Skip opening quote
		Lexer.Cursor.MoveToNextColumn();

		// Chain characters together until closing quote
		while (Lexer.CursorChar != '"')
		{
			value += Lexer.CursorChar;
			Lexer.Cursor.MoveToNextColumn();

			if (Lexer.AtEndOfFile || Lexer.CursorChar == '\n')
			{
				throw new LexicalException(startLine, startColumn, "String is missing closing quote '\"'");
			}
		}

		// Skip closing quote
		Lexer.Cursor.MoveToNextColumn();

		Lexer.Tokens.Add(new Token(TokenType.String, value, Lexer.Cursor.Line, startColumn));
	}
}