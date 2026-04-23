using Phases.Lexing;

namespace Tokens.Tokenization.Strategies;

public class ImportStrategy : ITokenizationStrategy
{
	public static bool TryTokenize(Lexer lexer)
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
		string localFilePath = "";

		// Chain characters together until closing quote
		while (lexer.CursorChar != '"')
		{
			localFilePath += lexer.CursorChar;
			lexer.Cursor.MoveToNextColumn();

			if (lexer.AtEndOfFile || lexer.CursorChar == '\n')
			{
				throw new LexicalException(startLine, startColumn, "Import statement string is missing closing quote '\"'");
			}
		}

		// Skip closing quote
		lexer.Cursor.MoveToNextColumn();

		string fullFilePath = Path.GetFullPath(Path.Combine(lexer.BaseDirectory, localFilePath));
		lexer.AddFile(startLine, startColumn, fullFilePath);

		return true;
	}
}

