using Phases.Lexing;

namespace Tokens.TokenizationStrategies;

public class TokenizeComment : ITokenizationStrategy
{
	public static bool TryTokenize(Lexer lexer)
	{
		if (lexer.CursorChar != '%')
		{
			return false;
		}

		int startLine = lexer.Cursor.Line;
		int startColumn = lexer.Cursor.Column;

		// Skip opening percentage
		lexer.Cursor.MoveToNextColumn();

		// Skip until closing percentage
		while (lexer.CursorChar != '%')
		{
			if (lexer.CursorChar == '\n')
			{
				lexer.Cursor.MoveToNewLine();
			}
			else
			{
				lexer.Cursor.MoveToNextColumn();
			}

			if (lexer.AtEndOfFile)
			{
				throw new LexicalException(startLine, startColumn, "Comment is missing closing percentage '%'");
			}
		}

		// Skip closing percentage
		lexer.Cursor.MoveToNextColumn();

		return true;
	}
}