using Phases.Lexing;

namespace Tokens.Tokenization.Strategies;

public class NumberOrMinusStrategy : ITokenizationStrategy
{
	public static bool TryTokenize(Lexer lexer)
	{
		if (lexer.CursorChar != '-' && !char.IsDigit(lexer.CursorChar))
		{
			return false;
		}

		string value = "";
		int startColumn = lexer.Cursor.Column;
		bool hasDecimalSymbol = false;

		// Allow a single '-' prefix
		if (lexer.CursorChar == '-')
		{
			value += lexer.CursorChar;
			lexer.Cursor.MoveToNextColumn();
		}

		// Chain numerical characters together
		while (!lexer.AtEndOfFile
			&& (char.IsDigit(lexer.CursorChar) || lexer.CursorChar == '.'))
		{
			if (lexer.CursorChar == '.')
			{
				if (hasDecimalSymbol)
				{
					throw new LexicalException(lexer.Cursor.Line, lexer.Cursor.Column, "Encountered multiple decimal symbols '.'");
				}
				hasDecimalSymbol = true;
			}

			value += lexer.CursorChar;
			lexer.Cursor.MoveToNextColumn();
		}

		if (value == "-") // If single minus symbol
		{
			lexer.Tokens.Add(new Token(TokenType.Minus, "", lexer.Cursor.Line, startColumn));
		}
		else if (hasDecimalSymbol) // If float
		{
			lexer.Tokens.Add(new Token(TokenType.Float, value, lexer.Cursor.Line, startColumn));
		}
		else  // If integer
		{
			lexer.Tokens.Add(new Token(TokenType.Integer, value, lexer.Cursor.Line, startColumn));
		}

		return true;
	}
}