

namespace LexicalAnalysis.Tokenization;

public interface ITokenizerStrategy
{
	public bool IsTokenizable(LexicalAnalyzer analyzer);
	public void Tokenize(LexicalAnalyzer analyzer);
}