using Lexing;

namespace Tokens.TokenizationStrategies;

public class TokenizeSlash : ITokenizationStrategy
{
	public static bool TryTokenize()
	{
		if (Lexer.CursorChar != '/')
		{
			return false;
		}

		Lexer.Tokens.Add(new Token(TokenType.Slash, "", Lexer.Cursor.Line, Lexer.Cursor.Column));
		Lexer.Cursor.MoveToNextColumn();

		return true;
	}
}

