using Phases.Lexing;

namespace Tokens.Tokenization.Strategies;

public class WhitespaceStrategy : ITokenizationStrategy
{
	public static bool TryTokenize(FileLexer lexer)
	{
		if (!char.IsWhiteSpace(lexer.CursorChar))
		{
			return false;
		}

		if (lexer.CursorChar == '\n')
		{
			// Add newline token
			lexer.AddToken(TokenType.Newline, "", lexer.Cursor.Line, lexer.Cursor.Column);
			lexer.Cursor.MoveToNewLine();

			// Check for following indent token
			int indentSize = 0;
			while (!lexer.AtEndOfFile && lexer.CursorChar == '\t')
			{
				indentSize++;
				lexer.Cursor.MoveToNextColumn();
			}
			if (indentSize != 0)
			{
				lexer.AddToken(TokenType.Indent, indentSize.ToString(), lexer.Cursor.Line, lexer.Cursor.Column);
			}
		}
		else // Ignore whitespace if not a newline
		{
			lexer.Cursor.MoveToNextColumn();
		}

		return true;
	}
}