using Phases.Lexing;

namespace Tokens.Tokenization.Strategies;

public class SlashStrategy : ITokenizationStrategy
{
	public static bool TryTokenize(FileLexer lexer)
	{
		if (lexer.CursorChar != '/')
		{
			return false;
		}

		lexer.AddToken(TokenType.Slash, "", lexer.Cursor.Line, lexer.Cursor.Column);
		lexer.Cursor.MoveToNextColumn();

		return true;
	}
}

