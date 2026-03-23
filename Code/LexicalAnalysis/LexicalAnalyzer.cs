using LexicalAnalysis.Lexers;

namespace LexicalAnalysis;

public class LexicalAnalyzer
{
	private List<string> _errors = new();
	private string _inputText = "";

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
			throw new Exception("Lexical errors:\n" + string.Join("\n- ", _errors));
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
			else if (CursorChar() == '#')
			{
				CommentLexer.Lex(this);
			}
			else if (CursorChar() == '"')
			{
				StringLexer.Lex(this);
			}
			else if (CursorChar() == '-')
			{
				HyphenLexer.Lex(this);
			}
			else if (CursorChar() == '(')
			{
				Tokens.Add(new Token(TokenType.LeftParen, "(", Cursor.Line, Cursor.Column));
				Cursor.MoveToNextColumn();
			}
			else if (CursorChar() == ')')
			{
				Tokens.Add(new Token(TokenType.RightParen, ")", Cursor.Line, Cursor.Column));
				Cursor.MoveToNextColumn();
			}
			else if (CursorChar() == ',')
			{
				Tokens.Add(new Token(TokenType.Comma, ",", Cursor.Line, Cursor.Column));
				Cursor.MoveToNextColumn();
			}
			else if (char.IsDigit(CursorChar()))
			{
				NumberLexer.Lex(this);
			}
			else if (CursorChar() == '_' || char.IsLetter(CursorChar()))
			{
				IdentifierOrKeywordLexer.Lex(this);
			}
			else
			{
				_errors.Add($"Unknown token type: Character: '{CursorChar}',  Line: {Cursor.Line},  Column: {Cursor.Column}");
				Cursor.MoveToNextColumn();
			}
		}

		Tokens.Add(new Token(TokenType.EndOfFile, "", Cursor.Line, Cursor.Column));
	}
}