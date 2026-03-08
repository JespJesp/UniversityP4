

namespace LexicalAnalysis;

public class LexicalAnalyzer
{
	public List<Token> Tokenize(string input)
	{
		TokenizationOperation operation = new(input);
		operation.Run();
		return operation.Tokens;
	}
}