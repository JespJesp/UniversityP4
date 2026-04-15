using Lexing;

namespace Tokens.TokenizationStrategies;

public class TokenizeNumber : ITokenizationStrategy
{
	public static bool TryTokenize()
	{
		if (Lexer.CursorChar != '-' && !char.IsDigit(Lexer.CursorChar))
		{
			return false;
		}

		string value = "";
		int startColumn = Lexer.Cursor.Column;
		bool hasDecimalSymbol = false;

		// Allow a single '-' prefix
		if (Lexer.CursorChar == '-')
		{
			value += Lexer.CursorChar;
			Lexer.Cursor.MoveToNextColumn();
		}

		// Chain numerical characters together
		while (!Lexer.AtEndOfFile 
			&& (char.IsDigit(Lexer.CursorChar) || Lexer.CursorChar == '.'))
		{
			if (Lexer.CursorChar == '.')
			{
				if (hasDecimalSymbol)
				{
					throw new LexicalException(Lexer.Cursor.Line, Lexer.Cursor.Column, "Encountered multiple decimal symbols '.'");
				}
				hasDecimalSymbol = true;
			}

			value += Lexer.CursorChar;
			Lexer.Cursor.MoveToNextColumn();
		}

		// Determine whether it is a float or an integer
		if (hasDecimalSymbol)
		{
			Lexer.Tokens.Add(new Token(TokenType.Float, value, Lexer.Cursor.Line, startColumn));
		}
		else
		{
			Lexer.Tokens.Add(new Token(TokenType.Integer, value, Lexer.Cursor.Line, startColumn));
		}

		return true;
	}
}