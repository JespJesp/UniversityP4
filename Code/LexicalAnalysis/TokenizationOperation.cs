using LexicalAnalysis.TokenParsing;
using LexicalAnalysis.TokenParsing.ParseStrategies;

namespace LexicalAnalysis;

public class TokenizationOperation
{
	internal List<Token> Tokens = new List<Token>();
	internal string Input = "";
	internal int CurrentLine;
	internal int CurrentColumn;
	internal int CurrentPosition;
	internal char CurrentChar;

	internal TokenizationOperation(string newInput)
	{
		Tokens.Clear();
		Input = newInput;
		CurrentLine = 1;
		CurrentColumn = 1;
		CurrentPosition = 0;
	}

	internal void Run()
	{
		while (CurrentPosition < Input.Length)
		{
			CurrentChar = Input[CurrentPosition];

			if (TokenParser.TryParse(new ParseWhitespace(), this)
				|| TokenParser.TryParse(new ParseNumber(), this)
				|| TokenParser.TryParse(new ParseIdentifierAndKeyword(), this)
				|| TokenParser.TryParse(new ParseString(), this)
				|| TokenParser.TryParse(new ParseHyphen(), this))
			{
				continue;
			}

			TokenParser.TryParse(new ParseUnknownCharacter(), this);
		}

		Tokens.Add(new Token(TokenType.EndOfFile, "", CurrentLine, CurrentColumn));
	}
}