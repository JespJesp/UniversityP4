using LexicalAnalysis.Tokenization;
using LexicalAnalysis.Tokenization.Strategies;

namespace LexicalAnalysis;

public class LexicalAnalyzer
{
	internal List<Token> Tokens = new List<Token>();
	internal string Input = "";
	internal int CurrentLine;
	internal int CurrentColumn;
	internal int CurrentPosition;
	internal char CurrentChar;

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

			if (Tokenizer.TryTokenize(new TokenizeWhitespace(), this)
				|| Tokenizer.TryTokenize(new TokenizeNumber(), this)
				|| Tokenizer.TryTokenize(new TokenizeIdentifierAndKeyword(), this)
				|| Tokenizer.TryTokenize(new TokenizeString(), this)
				|| Tokenizer.TryTokenize(new TokenizeHyphen(), this))
			{
				continue;
			}

			Tokenizer.TryTokenize(new TokenizeUnknownCharacter(), this);
		}

		Tokens.Add(new Token(TokenType.EndOfFile, "", CurrentLine, CurrentColumn));
	}
}