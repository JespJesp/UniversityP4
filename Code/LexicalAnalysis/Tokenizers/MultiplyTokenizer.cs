namespace LexicalAnalysis.Tokenizers;

public class MultiplyTokenizer : Tokenizer
{
	protected override bool IsTokenizable(LexicalAnalyzer a)
	{
		return a.CursorChar() == '*';
	}

	protected override void Tokenize(LexicalAnalyzer a)
	{
		a.Tokens.Add(
			new Token(
				TokenType.Multiply,
				"*",
				a.CursorLine,
				a.CursorColumn));

		a.AdvanceCursorToNextColumn();
	}
}