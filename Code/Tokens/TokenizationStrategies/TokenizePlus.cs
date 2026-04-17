using Phases.Lexing;

namespace Tokens.TokenizationStrategies;

public class TokenizePlus : ITokenizationStrategy
{
	public static bool TryTokenize(Lexer lexer)
	{
		if (lexer.CursorChar != '+')
		{
			return false;
		}

		lexer.Tokens.Add(new Token(TokenType.Plus, "", lexer.Cursor.Line, lexer.Cursor.Column));
		lexer.Cursor.MoveToNextColumn();

		return true;
	}
}

