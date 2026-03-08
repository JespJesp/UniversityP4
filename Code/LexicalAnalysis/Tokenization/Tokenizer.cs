

namespace LexicalAnalysis.Tokenization;

public static class Tokenizer
{
	public static bool TryTokenize(ITokenizerStrategy strategy, LexicalAnalyzer analyzer)
	{
		if (strategy.IsTokenizable(analyzer))
		{
			strategy.Tokenize(analyzer);
			return true;
		}
		return false;
	}
}