

namespace LexicalAnalysis.Tokenization;

public class TokenizeUnknownCharacter : Tokenizer
{
	protected override bool IsTokenizable(LexicalAnalyzer a)
	{
		return true; // Unknown characters are always parsed
	}

	protected override void Tokenize(LexicalAnalyzer a)
	{
		a.Tokens.Add(new Token(TokenType.Unknown, a.CursorChar().ToString(), a.CursorLine, a.CursorColumn));
		a.AdvanceCursor();
	}
}