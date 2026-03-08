

namespace LexicalAnalysis.Tokenization.Strategies;

public class TokenizeUnknownCharacter : ITokenizerStrategy
{
	public bool IsTokenizable(LexicalAnalyzer a)
	{
		return true; // Unknown characters are always parsed
	}

	public void Tokenize(LexicalAnalyzer a)
	{
		a.Tokens.Add(new Token(TokenType.Unknown, a.CurrentChar.ToString(), a.CurrentLine, a.CurrentColumn));
		a.CurrentPosition++;
		a.CurrentColumn++;
	}
}