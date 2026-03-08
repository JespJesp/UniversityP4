

namespace LexicalAnalysis.Tokenization;

public class TokenizeWhitespace : Tokenizer
{
	protected override bool IsTokenizable(LexicalAnalyzer a)
	{
		return char.IsWhiteSpace(a.CurrentChar);
	}

	protected override void Tokenize(LexicalAnalyzer a)
	{
		if (a.CurrentChar == '\n')
		{
			a.Tokens.Add(new Token(TokenType.NewLine, "\n", a.CurrentLine, a.CurrentColumn));
			a.AdvancePositionToNewLine();
		}
		else
		{
			a.AdvancePosition();
		}
	}
}