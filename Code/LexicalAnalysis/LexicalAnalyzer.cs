using LexicalAnalysis.Tokenizers;

namespace LexicalAnalysis;

public class LexicalAnalyzer
{
	internal List<Token> Tokens = new();
	internal List<string> Errors = new();
	internal string InputText = "";
	internal int CursorLine;
	internal int CursorColumn;
	internal int CursorPosition { get; private set; }
	internal char CursorChar() => InputText[CursorPosition];

	// Tokenizers
	WhitespaceTokenizer whitespaceTokenizer = new();
	IntegerTokenizer integerTokenizer = new();
	IdentifierOrKeywordTokenizer identifierOrKeywordTokenizer = new();
	StringTokenizer stringTokenizer = new();
	HyphenTokenizer hyphenTokenizer = new();

	public List<Token> Tokenize(string text)
	{
		// Reset variables
		Tokens.Clear();
		InputText = text;
		CursorLine = 1;
		CursorColumn = 1;
		CursorPosition = 0;

		TokenizeText();

		if (Errors.Any())
		{
			throw new Exception("Lexical errors:\n" + string.Join("\n", Errors));
		}

		return Tokens;
	}

	private void TokenizeText()
	{
		while (CursorPosition < InputText.Length)
		{
			if (whitespaceTokenizer.TryTokenize(this)
				|| integerTokenizer.TryTokenize(this)
				|| identifierOrKeywordTokenizer.TryTokenize(this)
				|| stringTokenizer.TryTokenize(this)
				|| hyphenTokenizer.TryTokenize(this))
			{
				continue;
			}

			Errors.Add($"Unknown token type: Starting character: '{CursorChar}',  Line: {CursorLine},  Column: {CursorColumn}");
			AdvanceCursor();
		}

		Tokens.Add(new Token(TokenType.EndOfFile, "", CursorLine, CursorColumn));
	}

	public void AdvanceCursor()
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