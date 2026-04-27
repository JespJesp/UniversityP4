using Phases.Lexing;

namespace Tokens.Tokenization.Strategies;

public class LeftParenthesesStrategy : ITokenizationStrategy
{
	public static bool TryTokenize(FileLexer lexer)
	{
		if (lexer.CursorChar != '(')
		{
			return false;
		}

		lexer.AddToken(TokenType.LeftParentheses, "", lexer.Cursor.Line, lexer.Cursor.Column);
		lexer.Cursor.MoveToNextColumn();

		return true;
	}
}

