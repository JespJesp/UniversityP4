using Lexing.Lexers;
using Lexing.Tokens;

namespace Lexing;

public static class Lexer
{
	private static string _inputText = "";
	private static List<LexicalError> _errors = new();

	public static List<Token> Tokens = new();
	public static LexicalAnalyzerCursor Cursor = new();

	public static char CursorChar => _inputText[Cursor.Position];
	public static bool IsNotEndOfFile => Cursor.Position < _inputText.Length;

	public static List<Token> Lex(string text)
	{
		// Reset variables
		Tokens.Clear();
		_errors.Clear();
		_inputText = text;
		Cursor.MoveToStartPosition();

		LexText();

		if (_errors.Any())
		{
			string errorMessage = "Lexical errors:";
			foreach (LexicalError error in _errors)
			{
				errorMessage += $"\n- Line: {error.Line}, Column: {error.Column}, Message: {error.Message}";
			}
			throw new Exception(errorMessage);
		}

		return Tokens;
	}

	private static void LexText()
	{
		// TODO: Add max size to e.g. float and string

		while (Cursor.Position < _inputText.Length)
		{
			// We use chained if-statements here instead of a switch,
			// because we not only look at the value of "CursorChar", 
			// but also need to use methods (such as "char.IsWhiteSpace()").
			// Switches can support methods using the "where" keyword, 
			// but this prevents the switch from acting as a jump table,
			// because the method cannot be treated as a constant,
			// so we'd gain no performance benefit from using the switch instead of if-statements.
			// Furthermore, the switch would be less readable because of the added syntax
			// needed to use the "where" keyword, so if-statements are more readable here.
			if (char.IsWhiteSpace(CursorChar))
			{
				WhitespaceLexer.Lex();
			}
			else if (CursorChar == '"')
			{
				StringLexer.Lex();
			}
			else if (CursorChar == '(')
			{
				Tokens.Add(new Token(TokenType.LeftParentheses, "", Cursor.Line, Cursor.Column));
				Cursor.MoveToNextColumn();
			}
			else if (CursorChar == ')')
			{
				Tokens.Add(new Token(TokenType.RightParentheses, "", Cursor.Line, Cursor.Column));
				Cursor.MoveToNextColumn();
			}
			else if (CursorChar == '/')
			{
				Tokens.Add(new Token(TokenType.ForwardSlash, "", Cursor.Line, Cursor.Column));
				Cursor.MoveToNextColumn();
			}
			else if (CursorChar == ',')
			{
				Tokens.Add(new Token(TokenType.Comma, "", Cursor.Line, Cursor.Column));
				Cursor.MoveToNextColumn();
			}
			else if (CursorChar == '-' || char.IsDigit(CursorChar))
			{
				NumberLexer.Lex();
			}
			else if (CursorChar == '_' || CursorChar == '#' || char.IsLetter(CursorChar))
			{
				IdentifierOrKeywordLexer.Lex();
			}
			else if (CursorChar == '%')
			{
				CommentLexer.Lex();
			}
			else
			{
				_errors.Add(new LexicalError(Cursor.Column, Cursor.Line, $"Unknown token type: Character: '{CursorChar}'"));
				Cursor.MoveToNextColumn();
			}
		}

		Tokens.Add(new Token(TokenType.EndOfFile, "", Cursor.Line, Cursor.Column));
	}

	public static void AddError(LexicalError error)
	{
		_errors.Add(error);
	}
}