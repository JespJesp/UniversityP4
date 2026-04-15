using Phases.Lexing;

namespace Tokens.TokenizationStrategies;

public class TokenizePlus : ITokenizationStrategy
{
	public static bool TryTokenize()
	{
		if (Lexer.CursorChar != '+')
		{
			return false;
		}

		Lexer.Tokens.Add(new Token(TokenType.Plus, "", Lexer.Cursor.Line, Lexer.Cursor.Column));
		Lexer.Cursor.MoveToNextColumn();

		return true;
	}
}

