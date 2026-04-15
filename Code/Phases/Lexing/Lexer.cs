using Tokens;
using Tokens.TokenizationStrategies;

namespace Lexing;

public static class Lexer
{
	public static List<Token> Tokens = new();
	public static LexerCursor Cursor = new();

	private static string _inputText = "";
	private static List<string> _lexicalErrors = new();

	public static char CursorChar => _inputText[Cursor.Position];
	public static bool AtEndOfFile => Cursor.Position >= _inputText.Length;

	public static List<Token> Lex(string text)
	{
		_inputText = text;

		LexInput();

		if (_lexicalErrors.Any())
		{
			throw new Exception("Lexical errors:\n- " + string.Join("\n- ", _lexicalErrors));
		}

		return Tokens;
	}

	private static void LexInput()
	{
		// TODO: Add max size to e.g. float and string

		while (Cursor.Position < _inputText.Length)
		{
			try
			{
				if (!Tokenizer.TryTokenize<TokenizeWhitespace>()
					&& !Tokenizer.TryTokenize<TokenizeComment>()
					&& !Tokenizer.TryTokenize<TokenizeString>()
					&& !Tokenizer.TryTokenize<TokenizeLeftParentheses>()
					&& !Tokenizer.TryTokenize<TokenizeRightParentheses>()
					&& !Tokenizer.TryTokenize<TokenizeComma>()
					&& !Tokenizer.TryTokenize<TokenizePlus>()
					&& !Tokenizer.TryTokenize<TokenizeAsterisk>()
					&& !Tokenizer.TryTokenize<TokenizeSlash>()
					&& !Tokenizer.TryTokenize<TokenizeNumber>()
					&& !Tokenizer.TryTokenize<TokenizeIdentifierOrKeyword>()
					)
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
}