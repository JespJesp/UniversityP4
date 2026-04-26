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

	public void Lex()
	{
		Cursor = new();

		// TODO: Add max size to e.g. float and string

		while (Cursor.Position < _fileContent.Length)
		{
			try
			{
				if (!Tokenizer.TryTokenize<WhitespaceStrategy>(this)
					&& !Tokenizer.TryTokenize<ImportStrategy>(this)
					&& !Tokenizer.TryTokenize<CommentStrategy>(this)
					&& !Tokenizer.TryTokenize<StringStrategy>(this)
					&& !Tokenizer.TryTokenize<LeftParenthesesStrategy>(this)
					&& !Tokenizer.TryTokenize<RightParenthesesStrategy>(this)
					&& !Tokenizer.TryTokenize<CommaStrategy>(this)
					&& !Tokenizer.TryTokenize<PlusStrategy>(this)
					&& !Tokenizer.TryTokenize<AsteriskStrategy>(this)
					&& !Tokenizer.TryTokenize<SlashStrategy>(this)
					&& !Tokenizer.TryTokenize<NumberOrMinusStrategy>(this)
					&& !Tokenizer.TryTokenize<IdentifierStrategy>(this))
				{
					Cursor.MoveToNextColumn();
					throw new LexicalError(Cursor.Line, Cursor.Column - 1, $"Unknown token type for character: '{CursorChar}'");
				}
			}
			catch (LexicalError exception)
			{
				CursorInfo cursorInfo = new(_fileName, exception.Line, exception.Column);
				_lexer.Errors.Add($"{cursorInfo}. {exception.Message}");
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

	public void LexNewFile(int callerLine, int callerColumn, string localFilePath)
	{
		// Get full path
		string fullFilePath;
		try
		{
			fullFilePath = Path.GetFullPath(Path.Combine(_lexer.BaseDirectory, localFilePath));
		}
		catch
		{
			throw new LexicalError(callerLine, callerColumn, $"Could not find file from local file path: {localFilePath}");
		}

		// Skip circular imports
		if (_lexer.ImportedFilePaths.Contains(fullFilePath))
		{
			return;
		}

		// Read file
		string fileContent = "";
		string fileName = "";
		try
		{
			fileContent = File.ReadAllText(fullFilePath);
			fileName = Path.GetFileName(fullFilePath);
		}
		catch
		{
			throw new LexicalError(callerLine, callerColumn, $"Could not find file from full path: {fullFilePath}");
		}

		// Remember file and lex it
		_lexer.ImportedFilePaths.Add(fullFilePath);
		new FileLexer(_lexer, fileName, fileContent).Lex();
	}
}