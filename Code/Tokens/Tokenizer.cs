using Tokens.TokenizationStrategies;

namespace Tokens;

public static class Tokenizer
{
	public static bool TryTokenize<T>() where T : ITokenizationStrategy
	{
		return T.TryTokenize();
	}
}