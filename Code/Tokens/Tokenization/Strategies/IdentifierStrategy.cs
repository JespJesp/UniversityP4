using Phases.Lexing;

namespace Tokens.Tokenization.Strategies;

public class IdentifierStrategy : ITokenizationStrategy
{
	public static bool TryTokenize(Lexer lexer)
	{
		if (lexer.CursorChar != '_' && !char.IsLetter(lexer.CursorChar))
		{
			return false;
		}

		string id = "";
		int startColumn = lexer.Cursor.Column;

		// Chain characters together
		while (!lexer.AtEndOfFile
			&& (lexer.CursorChar == '_' || char.IsLetterOrDigit(lexer.CursorChar)))
		{
			id += lexer.CursorChar;
			lexer.Cursor.MoveToNextColumn();
		}

		lexer.Tokens.Add(new Token(TokenType.Identifier, id, lexer.Cursor.Line, startColumn));

		return true;
	}
}