using Tokens;
using Tokens.TokenizationStrategies;

namespace Phases.Lexing;

public class Lexer
{
	public List<Token> Tokens = new();
	public LexerCursor Cursor = new();

	private string _inputText = "";
	private List<string> _lexicalErrors = new();

	public char CursorChar => _inputText[Cursor.Position];
	public bool AtEndOfFile => Cursor.Position >= _inputText.Length;

	public List<Token> Lex(string text)
	{
		_inputText = text;

		LexInput();

		if (_lexicalErrors.Any())
		{
			throw new Exception("Lexical errors:\n- " + string.Join("\n- ", _lexicalErrors));
		}

		return Tokens;
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
					throw new LexicalException(Cursor.Line, Cursor.Column - 1, $"Unknown token type for character: '{CursorChar}'.");
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
		return Tokenizer.TryTokenize<TokenizeWhitespace>(this)
			|| Tokenizer.TryTokenize<TokenizeComment>(this)
			|| Tokenizer.TryTokenize<TokenizeString>(this)
			|| Tokenizer.TryTokenize<TokenizeLeftParentheses>(this)
			|| Tokenizer.TryTokenize<TokenizeRightParentheses>(this)
			|| Tokenizer.TryTokenize<TokenizeComma>(this)
			|| Tokenizer.TryTokenize<TokenizePlus>(this)
			|| Tokenizer.TryTokenize<TokenizeAsterisk>(this)
			|| Tokenizer.TryTokenize<TokenizeSlash>(this)
			|| Tokenizer.TryTokenize<TokenizeNumber>(this)
			|| Tokenizer.TryTokenize<TokenizeIdentifierOrKeyword>(this);
	}
}