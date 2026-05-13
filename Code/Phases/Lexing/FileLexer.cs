using Tokens;
using Tokens.Tokenization;
using Tokens.Tokenization.Strategies;

namespace Phases.Lexing;

public class FileLexer
{
	public Cursor Cursor = new();
	private Lexer _lexer;
	private string _fileName;
	private string _fileContent;

	public char CursorChar => _fileContent[Cursor.Position];
	public bool AtEndOfFile => Cursor.Position >= _fileContent.Length;

	public FileLexer(Lexer lexer, string fileName, string fileContent)
	{
		_fileName = fileName;
		_fileContent = fileContent;
		_lexer = lexer;
	}

	public void Lex(string fileFullPath)
	{
		_lexer.ImportedFileFullPaths.Add(fileFullPath);
		Cursor = new();

		// TODO: Add max size to e.g. float and string

		while (Cursor.Position < _fileContent.Length)
		{
			try
			{
				if (!Tokenizer.TryTokenize(this))
				{
					Cursor.MoveToNextColumn();
					throw new LexicalError(Cursor.Line, Cursor.Column - 1, $"Unknown token type for character: '{CursorChar}'");
				}
			}
			catch (LexicalError exception)
			{
				Location location = new(_fileName, exception.Line, exception.Column);
				_lexer.Errors.Add($"{location}. {exception.Message}");
			}
		}
	}

	public void AddToken(TokenType type, string value, int line, int column)
	{
		Token token = new(type, value, _fileName, line, column);
		_lexer.Tokens.Add(token);
	}

	public bool ExpectString(string expected)
	{
		string lookaheadCharacters = "";
		for (int i = 0; i < expected.Length; i++)
		{
			// Return if at end of file
			if (Cursor.Position + i >= _fileContent.Length)
			{
				break;
			}

			lookaheadCharacters += _fileContent[Cursor.Position + i];
		}

		return lookaheadCharacters == expected;
	}

	public void LexNewFile(int callerLine, int callerColumn, string fileLocalPath)
	{
		// Get full path
		string fileFullPath;
		try
		{
			fileFullPath = Path.GetFullPath(Path.Combine(_lexer.InputFileFolderFullPath, fileLocalPath));
		}
		catch
		{
			throw new LexicalError(callerLine, callerColumn, $"Could not find file from local file path: {fileLocalPath}");
		}

		// Skip circular imports
		if (_lexer.ImportedFileFullPaths.Contains(fileFullPath))
		{
			return;
		}

		// Find file
		string fileContent = "";
		string fileName = "";
		try
		{
			fileContent = File.ReadAllText(fileFullPath);
			fileName = Path.GetFileName(fileFullPath);
		}
		catch
		{
			throw new LexicalError(callerLine, callerColumn, $"Could not find file from full path: {fileFullPath}");
		}

		new FileLexer(_lexer, fileName, fileContent).Lex(fileFullPath);
	}
}