

namespace LexicalAnalysis.Tokenization.Strategies;

public class TokenizeWhitespace : ITokenizerStrategy
{
	public bool IsTokenizable(LexicalAnalyzer a)
	{
		return char.IsWhiteSpace(a.CurrentChar);
	}

	public void Tokenize(LexicalAnalyzer a)
	{
		if (a.CurrentChar == '\n')
		{
			a.Tokens.Add(new Token(TokenType.NewLine, "\n", a.CurrentLine, a.CurrentColumn));
			a.CurrentLine++;
			a.CurrentColumn = 1;
		}
		else
		{
			a.CurrentColumn++;
		}
		a.CurrentPosition++;
	}
}