using Phases.Lexing;

namespace Tokens.TokenizationStrategies;

public class TokenizeRightParentheses : ITokenizationStrategy
{
	public static bool TryTokenize(Lexer lexer)
	{
		if (lexer.CursorChar != ')')
		{
			return false;
		}

		lexer.Tokens.Add(new Token(TokenType.RightParentheses, "", lexer.Cursor.Line, lexer.Cursor.Column));
		lexer.Cursor.MoveToNextColumn();

		return true;
	}
}

