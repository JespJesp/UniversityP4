

namespace LexicalAnalysis.Tokenization.Strategies;

public class TokenizeHyphen : ITokenizerStrategy
{
	public bool IsTokenizable(LexicalAnalyzer a)
	{
		return a.CurrentChar == '-';
	}

	public void Tokenize(LexicalAnalyzer a)
	{
		a.Tokens.Add(new Token(TokenType.Hyphen, "-", a.CurrentLine, a.CurrentColumn));
		a.CurrentPosition++;
		a.CurrentColumn++;
	}
}