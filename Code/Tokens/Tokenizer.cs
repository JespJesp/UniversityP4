using Tokens.TokenizationStrategies;

namespace Tokens; //TODO: Fix the namespaces after my restructuring

public static class Tokenizer
{
	public static bool TryTokenize<T>() where T : ITokenizationStrategy
	{
		return T.TryTokenize();
	}
}