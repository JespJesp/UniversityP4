using Phases.Lexing;

namespace Tokens.Tokenization.Strategies;

public class ImportStrategy : ITokenizationStrategy
{
	public static bool TryTokenize(FileLexer lexer)
	{
		// Check for import statement
		string importStatementStart = "import \"";
		if (!lexer.ExpectString(importStatementStart))
		{
			return false;
		}
		for (int i = 0; i < importStatementStart.Length; i++)
		{
			lexer.Cursor.MoveToNextColumn();
		}

		int startLine = lexer.Cursor.Line;
		int startColumn = lexer.Cursor.Column;
		string fileLocalPath = "";

		// Chain characters together until closing quote
		while (lexer.CursorChar != '"')
		{
			fileLocalPath += lexer.CursorChar;
			lexer.Cursor.MoveToNextColumn();

			if (lexer.AtEndOfFile || lexer.CursorChar == '\n')
			{
				throw new LexicalError(startLine, startColumn, "Import statement string is missing closing quote '\"'");
			}
		}

		// Skip closing quote
		lexer.Cursor.MoveToNextColumn();

		lexer.LexNewFile(startLine, startColumn, fileLocalPath);
		lexer.AddToken(TokenType.EndOfImportedFile, "", lexer.Cursor.Line, lexer.Cursor.Column);

		return true;
	}
}

