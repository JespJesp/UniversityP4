using LexicalAnalysis.Tokenization;

namespace LexicalAnalysis;

public class LexicalAnalyzer
{
	internal List<Token> Tokens = new List<Token>();
	internal string Input = "";
	internal int CurrentLine;
	internal int CurrentColumn;
	internal int CurrentPosition;
	internal char CurrentChar;

	// Tokenizers
	TokenizeWhitespace tokenizeWhitespace = new();
	TokenizeNumber tokenizeNumber = new();
	TokenizeIdentifierAndKeyword tokenizeIdentifierAndKeyword = new();
	TokenizeString tokenizeString = new();
	TokenizeHyphen tokenizeHyphen = new();
	TokenizeUnknownCharacter tokenizeUnknownCharacter = new();

	public List<Token> Analyze(string input)
	{
		Tokens.Clear();
		Input = input;
		CurrentLine = 1;
		CurrentColumn = 1;
		CurrentPosition = 0;

		TokenizeInput();

		return Tokens;
	}

	private void TokenizeInput()
	{
		while (CurrentPosition < Input.Length)
		{
			CurrentChar = Input[CurrentPosition];

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

		Tokens.Add(new Token(TokenType.EndOfFile, "", CurrentLine, CurrentColumn));
	}

	public void AdvancePosition()
	{
		CurrentPosition++;
		CurrentColumn++;
	}

	public void AdvancePositionToNewLine()
	{
		CurrentPosition++;
		CurrentLine++;
		CurrentColumn = 1;
	}
}