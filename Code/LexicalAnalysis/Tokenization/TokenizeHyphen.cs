

namespace LexicalAnalysis.Tokenization;

public class TokenizeHyphen : Tokenizer
{
	protected override bool IsTokenizable(LexicalAnalyzer a)
	{
		return a.CurrentChar == '-';
	}

	protected override void Tokenize(LexicalAnalyzer a)
	{
		a.Tokens.Add(new Token(TokenType.Hyphen, "-", a.CurrentLine, a.CurrentColumn));
		a.AdvancePosition();
	}
}