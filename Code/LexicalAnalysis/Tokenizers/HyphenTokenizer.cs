

namespace LexicalAnalysis.Tokenizers;

public class HyphenTokenizer : Tokenizer
{
	protected override bool IsTokenizable(LexicalAnalyzer a)
	{
		return a.CursorChar() == '-';
	}

	protected override void Tokenize(LexicalAnalyzer a)
	{
		a.Tokens.Add(new Token(TokenType.Hyphen, "-", a.CursorLine, a.CursorColumn));
		a.AdvanceCursor();
	}
}