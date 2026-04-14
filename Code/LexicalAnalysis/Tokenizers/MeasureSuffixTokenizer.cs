namespace LexicalAnalysis.Tokenizers;

public class MeasureSuffixTokenizer : Tokenizer
{
	protected override bool IsTokenizable(LexicalAnalyzer a)
	{
		return a.CursorChar() == 'm';
	}

	protected override void Tokenize(LexicalAnalyzer a)
	{
		a.Tokens.Add(
			new Token(
				TokenType.MeasureSuffix,
				"m",
				a.CursorLine,
				a.CursorColumn));

		a.AdvanceCursorToNextColumn();
	}
}