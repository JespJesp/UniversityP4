using LexicalAnalysis.Tokenization;

namespace LexicalAnalysis;

public class LexicalAnalyzer
{
	internal List<Token> Tokens = new List<Token>();
	internal string Input = "";
	internal int CursorLine;
	internal int CursorColumn;
	internal int CursorPosition;
	internal char CursorChar() => Input[CursorPosition];

	// Tokenizers
	TokenizeWhitespace tokenizeWhitespace = new();
	TokenizeNumber tokenizeNumber = new();
	TokenizeIdentifierOrKeyword tokenizeIdentifierAndKeyword = new();
	TokenizeString tokenizeString = new();
	TokenizeHyphen tokenizeHyphen = new();
	TokenizeUnknownCharacter tokenizeUnknownCharacter = new();

	public List<Token> Analyze(string input)
	{
		Tokens.Clear();
		Input = input;
		CursorLine = 1;
		CursorColumn = 1;
		CursorPosition = 0;

		TokenizeInput();

		return Tokens;
	}

	private void TokenizeInput()
	{
		while (CursorPosition < Input.Length)
		{
			if (tokenizeWhitespace.TryTokenize(this)
				|| tokenizeNumber.TryTokenize(this)
				|| tokenizeIdentifierAndKeyword.TryTokenize(this)
				|| tokenizeString.TryTokenize(this)
				|| tokenizeHyphen.TryTokenize(this))
			{
				continue;
			}

			tokenizeUnknownCharacter.TryTokenize(this);
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