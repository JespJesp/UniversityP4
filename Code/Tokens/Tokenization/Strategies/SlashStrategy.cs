using Phases.Lexing;

namespace Tokens.Tokenization.Strategies;

public class SlashStrategy : ITokenizationStrategy
{
	public static bool TryTokenize(Lexer lexer)
	{
		if (lexer.CursorChar != '/')
		{
			return false;
		}

		lexer.Tokens.Add(new Token(TokenType.Slash, "", lexer.Cursor.Line, lexer.Cursor.Column));
		lexer.Cursor.MoveToNextColumn();

		return true;
	}
}

