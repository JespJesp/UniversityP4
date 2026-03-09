

namespace LexicalAnalysis.Tokenizers;

public abstract class Tokenizer
{
	protected abstract bool IsTokenizable(LexicalAnalyzer analyzer);
	protected abstract void Tokenize(LexicalAnalyzer analyzer);

	/// <returns>True if tokenization was applicable to the current char.</returns>
	public bool TryTokenize(LexicalAnalyzer analyzer)
	{
		if (IsTokenizable(analyzer))
		{
			Tokenize(analyzer);
			return true;
		}
		return false;
	}
}