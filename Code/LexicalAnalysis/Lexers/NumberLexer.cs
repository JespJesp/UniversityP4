namespace LexicalAnalysis.Lexers;

public static class NumberLexer
{
	public static void Lex(LexicalAnalyzer a)
	{
		string value = "";
		int startColumn = a.Cursor.Column;
		bool hasDecimalSymbol = false;

		// Chain characters together
		while (a.IsNotEndOfFile() && char.IsDigit(a.CursorChar()))
		{
			if (a.CursorChar() == '.')
			{
				if (hasDecimalSymbol)
				{
					throw new Exception($"Encountered multiple decimal symbols '.' while tokenizing float at Line: {a.Cursor.Line}, Column: {a.Cursor.Column}");
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