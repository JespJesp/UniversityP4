using Phases.Lexing;

namespace Tokens.Tokenization.Strategies;

public class PlusStrategy : ITokenizationStrategy
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

