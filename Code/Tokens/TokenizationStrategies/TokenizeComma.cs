using Lexing;

namespace Tokens.TokenizationStrategies;

public class TokenizeComma : ITokenizationStrategy
{
	public static bool TryTokenize()
	{
		if (Lexer.CursorChar != ',')
		{
			return false;
		}

		Lexer.Tokens.Add(new Token(TokenType.Comma, "", Lexer.Cursor.Line, Lexer.Cursor.Column));
		Lexer.Cursor.MoveToNextColumn();

		return true;
	}
}

