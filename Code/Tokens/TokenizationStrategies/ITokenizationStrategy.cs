namespace Tokens.TokenizationStrategies; //TODO: Fix the namespaces after my restructuring

public interface ITokenizationStrategy
{
	static abstract bool TryTokenize();
}