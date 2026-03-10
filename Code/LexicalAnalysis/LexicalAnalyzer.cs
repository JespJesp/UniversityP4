using LexicalAnalysis.Lexers;

namespace LexicalAnalysis;

public class LexicalAnalyzer
{
	private List<string> _errors = new();
	private string _inputText = "";

	public List<Token> Tokens = new();
	public int CursorLine { get; private set; } // Private set because the "AdvanceCursorX" methods handle set
	public int CursorColumn { get; private set; } // Private set because the "AdvanceCursorX" methods handle set
	public int CursorPosition { get; private set; } // Private set because the "AdvanceCursorX" methods handle set
	public char CursorChar() => _inputText[CursorPosition];

	public bool IsNotEndOfFile() => CursorPosition < _inputText.Length;

	public List<Token> Tokenize(string text)
	{
		// Reset variables
		Tokens.Clear();
		_inputText = text;
		CursorLine = 1;
		CursorColumn = 1;
		CursorPosition = 0;

		TokenizeText();

		if (_errors.Any())
		{
			throw new Exception("Lexical errors:\n" + string.Join("\n- ", _errors));
		}

		return Tokens;
	}

	private void TokenizeText()
	{
		// TODO: Add max size to e.g. float and string

		while (CursorPosition < _inputText.Length)
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
				_errors.Add($"Unknown token type: Character: '{CursorChar}',  Line: {CursorLine},  Column: {CursorColumn}");
				AdvanceCursorToNextColumn();
			}
		}

		Tokens.Add(new Token(TokenType.EndOfFile, "", CursorLine, CursorColumn));
	}

	public void AdvanceCursorToNextColumn()
	{
		CursorPosition++;
		CursorColumn++;
	}

	public void AdvanceCursorToNewLine()
	{
		CursorPosition++;
		CursorLine++;
		CursorColumn = 1;
	}
}