using LexicalAnalysis.Tokenizers;

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

	// Tokenizers
	private WhitespaceTokenizer _whitespaceTokenizer = new();
	private IntegerTokenizer _integerTokenizer = new();
	private IdentifierOrKeywordTokenizer _identifierOrKeywordTokenizer = new();
	private StringTokenizer _stringTokenizer = new();
	private HyphenTokenizer _hyphenTokenizer = new();

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
		while (CursorPosition < _inputText.Length)
		{
			if (_whitespaceTokenizer.TryTokenize(this)
				|| _integerTokenizer.TryTokenize(this)
				|| _identifierOrKeywordTokenizer.TryTokenize(this)
				|| _stringTokenizer.TryTokenize(this)
				|| _hyphenTokenizer.TryTokenize(this))
			{
				continue;
			}

			_errors.Add($"Unknown token type: Character: '{CursorChar}',  Line: {CursorLine},  Column: {CursorColumn}");
			AdvanceCursorToNextColumn();
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