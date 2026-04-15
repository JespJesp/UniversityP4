using Lexing;

namespace Tokens.TokenizationStrategies;

public class TokenizeWhitespace : ITokenizationStrategy
{
	public static bool TryTokenize()
	{
		if (!char.IsWhiteSpace(Lexer.CursorChar))
		{
			return false;
		}

		if (Lexer.CursorChar == '\n')
		{
			// Add newline token
			Lexer.Tokens.Add(new Token(TokenType.Newline, "", Lexer.Cursor.Line, Lexer.Cursor.Column));
			Lexer.Cursor.MoveToNewLine();

			// Check for following indent token
			int indentSize = 0;
			while (!Lexer.AtEndOfFile && Lexer.CursorChar == '\t')
			{
				indentSize++;
				Lexer.Cursor.MoveToNextColumn();
			}
			if (indentSize != 0)
			{
				Lexer.Tokens.Add(new Token(TokenType.Indent, indentSize.ToString(), Lexer.Cursor.Line, Lexer.Cursor.Column));
			}
		}
		else // Ignore whitespace if not a newline
		{
			Lexer.Cursor.MoveToNextColumn();
		}

		return true;
	}
}