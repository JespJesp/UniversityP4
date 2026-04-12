using LexicalAnalysis.Lexers;

namespace LexicalAnalysis;

public class LexicalAnalyzer
{
	private string _inputText = "";
	private List<LexicalError> _errors = new();

	public List<Token> Tokens = new();
	public LexicalAnalyzerCursor Cursor = new();

	public char CursorChar() => _inputText[Cursor.Position];
	public bool IsNotEndOfFile() => Cursor.Position < _inputText.Length;

	public List<Token> Lex(string text)
	{
		// Reset variables
		Tokens.Clear();
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

	private void LexText()
	{
		// TODO: Add max size to e.g. float and string

		while (Cursor.Position < _inputText.Length)
		{
			if (char.IsWhiteSpace(CursorChar()))
			{
				WhitespaceLexer.Lex(this);
			}
			else if (CursorChar() == '"')
			{
				StringLexer.Lex(this);
			}
			else if (CursorChar() == '(')
			{
				Tokens.Add(new Token(TokenType.LeftParentheses, "(", Cursor.Line, Cursor.Column));
				Cursor.MoveToNextColumn();
			}
			else if (CursorChar() == ')')
			{
				Tokens.Add(new Token(TokenType.RightParentheses, ")", Cursor.Line, Cursor.Column));
				Cursor.MoveToNextColumn();
			}
			else if (CursorChar() == ',')
			{
				Tokens.Add(new Token(TokenType.Comma, ",", Cursor.Line, Cursor.Column));
				Cursor.MoveToNextColumn();
			}
			else if (CursorChar() == '/')
			{
				ForwardSlashLexer.Lex(this);
			}
			else if (CursorChar() == '-' || char.IsDigit(CursorChar()))
			{
				NumberLexer.Lex(this);
			}
			else if (CursorChar() == '_' || CursorChar() == '#' || char.IsLetter(CursorChar()))
			{
				IdentifierOrKeywordLexer.Lex(this);
			}
			else if (CursorChar() == '%')
			{
				CommentLexer.Lex(this);
			}
			else
			{
				_errors.Add(new LexicalError(Cursor.Column, Cursor.Line, $"Unknown token type: Character: '{CursorChar()}'"));
				Cursor.MoveToNextColumn();
			}
		}

		Tokens.Add(new Token(TokenType.EndOfFile, "", Cursor.Line, Cursor.Column));
	}

	public void AddError(LexicalError error)
	{
		_errors.Add(error);
	}
}