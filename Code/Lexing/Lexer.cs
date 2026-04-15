using Lexing.Lexers;
using Lexing.Tokens;

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
				/*	
				Explanation: Why use chained if-statements instead of a switch here?
					Because we not only look at the value of "CursorChar", 
					but also need to use methods (such as "char.IsWhiteSpace()").
					Switches can support methods using the "where" keyword, 
					but this prevents the switch from acting as a jump table,
					because the method cannot be treated as a constant,
					so we'd gain no performance benefit from using the switch instead of if-statements.
					Furthermore, the switch would be less readable because of the added syntax
					needed to use the "where" keyword, so if-statements are more readable here.
				*/
				
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
				else if (CursorChar == ',')
				{
					Tokens.Add(new Token(TokenType.Comma, "", Cursor.Line, Cursor.Column));
					Cursor.MoveToNextColumn();
				}
				else if (CursorChar == '+')
				{
					Tokens.Add(new Token(TokenType.Plus, "", Cursor.Line, Cursor.Column));
					Cursor.MoveToNextColumn();
				}
				else if (CursorChar == '*')
				{
					Tokens.Add(new Token(TokenType.Asterisk, "", Cursor.Line, Cursor.Column));
					Cursor.MoveToNextColumn();
				}
				else if (CursorChar == '/')
				{
					Tokens.Add(new Token(TokenType.Slash, "", Cursor.Line, Cursor.Column));
					Cursor.MoveToNextColumn();
				}
				else if (CursorChar == '-' || char.IsDigit(CursorChar))
				{
					NumberLexer.Lex();
				}
				else if (CursorChar == '_' || char.IsLetter(CursorChar))
				{
					IdentifierOrKeywordLexer.Lex();
				}
				else if (CursorChar == '%')
				{
					CommentLexer.Lex();
				}
				else
				{
					Cursor.MoveToNextColumn();
					throw new LexicalException(Cursor.Column - 1, Cursor.Line, $"Unknown token type for character: '{CursorChar}'.");
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