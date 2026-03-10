namespace LexicalAnalysis.Lexers;

public static class NumberLexer
{
	public static void Lex(LexicalAnalyzer a)
	{
		string value = "";
		int startColumn = a.CursorColumn;
		bool hasDecimalSymbol = false;

		while (a.IsNotEndOfFile() && char.IsDigit(a.CursorChar()))
		{
			if (a.CursorChar() == '.')
			{
				if (hasDecimalSymbol)
				{
					throw new Exception($"Encountered multiple decimal symbols '.' while tokenizing float at Line: {a.CursorLine}, Column: {a.CursorColumn}");
				}
				hasDecimalSymbol = true;
			}

			value += a.CursorChar();
			a.AdvanceCursorToNextColumn();
		}

		if (hasDecimalSymbol)
		{
			a.Tokens.Add(new Token(TokenType.Float, value, a.CursorLine, startColumn));
		}
		else
		{
			a.Tokens.Add(new Token(TokenType.Integer, value, a.CursorLine, startColumn));
		}
	}
}