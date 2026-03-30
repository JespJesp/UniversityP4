namespace LexicalAnalysis.Lexers;

public static class NumberLexer
{
	public static void Lex(LexicalAnalyzer a)
	{
		string value = "";
		int startColumn = a.Cursor.Column;
		bool hasDecimalSymbol = false;

		if (a.CursorChar() == '-')
		{
			value += a.CursorChar();
			a.Cursor.MoveToNextColumn();
		}

		// Chain characters together
		while (a.IsNotEndOfFile() && (char.IsDigit(a.CursorChar()) || a.CursorChar() == '.'))
		{
			if (a.CursorChar() == '.')
			{
				if (hasDecimalSymbol)
				{
					a.AddError(new LexicalError(a.Cursor.Line, a.Cursor.Column, "Encountered multiple decimal symbols '.'"));
					return;
				}
				hasDecimalSymbol = true;
			}

			value += a.CursorChar();
			a.Cursor.MoveToNextColumn();
		}

		// Determine if it is a float or an integer
		if (hasDecimalSymbol)
		{
			a.Tokens.Add(new Token(TokenType.Float, value, a.Cursor.Line, startColumn));
		}
		else
		{
			a.Tokens.Add(new Token(TokenType.Integer, value, a.Cursor.Line, startColumn));
		}
	}
}