

namespace LexicalAnalysis.Tokenization;

public class TokenizeWhitespace : Tokenizer
{
	protected override bool IsTokenizable(LexicalAnalyzer a)
	{
		return char.IsWhiteSpace(a.CursorChar());
	}

	protected override void Tokenize(LexicalAnalyzer a)
	{
		if (a.CursorChar() == '\n')
		{
			a.Tokens.Add(new Token(TokenType.NewLine, "\n", a.CursorLine, a.CursorColumn));
			a.AdvanceCursorToNewLine();
		}
		else
		{
			a.AdvanceCursor();
		}
	}
}