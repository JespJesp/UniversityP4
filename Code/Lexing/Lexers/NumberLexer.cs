using Lexing.Tokens;

namespace Lexing.Lexers;

public static class NumberLexer
{
	public static void Lex()
	{
		string value = "";
		int startColumn = Lexer.Cursor.Column;
		bool hasDecimalSymbol = false;

		if (Lexer.CursorChar == '-')
		{
			value += Lexer.CursorChar;
			Lexer.Cursor.MoveToNextColumn();
		}

		// Chain characters together
		while (Lexer.IsNotEndOfFile && (char.IsDigit(Lexer.CursorChar) || Lexer.CursorChar == '.'))
		{
			if (Lexer.CursorChar == '.')
			{
				if (hasDecimalSymbol)
				{
					Lexer.AddError(new LexicalError(Lexer.Cursor.Line, Lexer.Cursor.Column, "Encountered multiple decimal symbols '.'"));
					return;
				}
				hasDecimalSymbol = true;
			}

			value += Lexer.CursorChar;
			Lexer.Cursor.MoveToNextColumn();
		}

		// Determine if it is a float or an integer
		if (hasDecimalSymbol)
		{
			Lexer.Tokens.Add(new Token(TokenType.Float, value, Lexer.Cursor.Line, startColumn));
		}
		else
		{
			Lexer.Tokens.Add(new Token(TokenType.Integer, value, Lexer.Cursor.Line, startColumn));
		}
	}
}