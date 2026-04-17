using Phases.Lexing;

namespace Tokens.Tokenization.Strategies;

public class StringStrategy : ITokenizationStrategy
{
	public static bool TryTokenize(Lexer lexer)
	{
		if (lexer.CursorChar != '"')
		{
			return false;
		}

		string value = "";
		int startLine = lexer.Cursor.Line;
		int startColumn = lexer.Cursor.Column;

		// Skip opening quote
		lexer.Cursor.MoveToNextColumn();

		// Chain characters together until closing quote
		while (lexer.CursorChar != '"')
		{
			value += lexer.CursorChar;
			lexer.Cursor.MoveToNextColumn();

			if (lexer.AtEndOfFile || lexer.CursorChar == '\n')
			{
				throw new LexicalException(startLine, startColumn, "String is missing closing quote '\"'");
			}
		}

		// Skip closing quote
		lexer.Cursor.MoveToNextColumn();

		lexer.Tokens.Add(new Token(TokenType.String, value, lexer.Cursor.Line, startColumn));

		return true;
	}
}