using Phases.Lexing;

namespace Tokens.Tokenization.Strategies;

public class RightParenthesesStrategy : ITokenizationStrategy
{
	public static bool TryTokenize(FileLexer lexer)
	{
		if (lexer.CursorChar != ')')
		{
			return false;
		}

		lexer.AddToken(TokenType.RightParentheses, "", lexer.Cursor.Line, lexer.Cursor.Column);
		lexer.Cursor.MoveToNextColumn();

		return true;
	}
}

