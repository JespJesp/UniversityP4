using Tokens;
using Tokens.Tokenization;
using Tokens.Tokenization.Strategies;

namespace Phases.Lexing;

public class Lexer
{
	public List<Token> Tokens = new();
	public LexerCursor Cursor = new();
	public string BaseDirectory { get; private set; } = "";
	public List<string> ImportedFilePaths = new();
	private string _inputText = "";
	private List<string> _lexicalErrors = new();

	public char CursorChar => _inputText[Cursor.Position];
	public bool AtEndOfFile => Cursor.Position >= _inputText.Length;

	public List<Token> Lex(string text, string baseDirectory)
	{
		_lexicalErrors.Clear();
		Tokens.Clear();
		Cursor = new LexerCursor();
		BaseDirectory = baseDirectory;
		_inputText = text;

		LexInput();

		if (_lexicalErrors.Any())
		{
			throw new Exception("Lexical errors:\n- " + string.Join("\n- ", _lexicalErrors));
		}

		return Tokens;
	}

	public bool ExpectString(string expected)
	{
		string lookaheadCharacters = "";
		for (int i = 0; i < expected.Length; i++)
		{
			// Return if at end of file
			if (Cursor.Position + i >= _inputText.Length)
			{
				break;
			}

			lookaheadCharacters += _inputText[Cursor.Position + i];
		}

		return lookaheadCharacters == expected;
	}

	public void AddFile(int callerLine, int callerColumn, string filePath)
	{
		// Skip circular imports
		if (ImportedFilePaths.Contains(filePath))
		{
			return;
		}

		string fileContent = "";
		try
		{
			fileContent = File.ReadAllText(filePath);
		}
		catch
		{
			throw new LexicalException(callerLine, callerColumn, $"Could not find file: {filePath}");
		}

		ImportedFilePaths.Add(filePath);
		_inputText = _inputText.Insert(Cursor.Position, fileContent);
	}
	
	private void LexInput()
	{
		// TODO: Add max size to e.g. float and string

		while (Cursor.Position < _inputText.Length)
		{
			try
			{
				if (!TryTokenizeChar())
				{
					Cursor.MoveToNextColumn();
					throw new LexicalException(Cursor.Line, Cursor.Column - 1, $"Unknown token type for character: '{CursorChar}'");
				}
			}
			catch (LexicalException exception)
			{
				_lexicalErrors.Add(exception.Message);
			}
		}

		Tokens.Add(new Token(TokenType.EndOfFile, "", Cursor.Line, Cursor.Column));
	}

	private bool TryTokenizeChar()
	{
		return Tokenizer.TryTokenize<WhitespaceStrategy>(this)
			|| Tokenizer.TryTokenize<ImportStrategy>(this)
			|| Tokenizer.TryTokenize<CommentStrategy>(this)
			|| Tokenizer.TryTokenize<StringStrategy>(this)
			|| Tokenizer.TryTokenize<LeftParenthesesStrategy>(this)
			|| Tokenizer.TryTokenize<RightParenthesesStrategy>(this)
			|| Tokenizer.TryTokenize<CommaStrategy>(this)
			|| Tokenizer.TryTokenize<PlusStrategy>(this)
			|| Tokenizer.TryTokenize<AsteriskStrategy>(this)
			|| Tokenizer.TryTokenize<SlashStrategy>(this)
			|| Tokenizer.TryTokenize<NumberOrMinusStrategy>(this)
			|| Tokenizer.TryTokenize<IdentifierStrategy>(this);
	}
}