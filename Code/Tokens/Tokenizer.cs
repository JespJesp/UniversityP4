using Phases.Lexing;
using Tokens.TokenizationStrategies;

namespace Tokens;

public static class Tokenizer
{
	public static bool TryTokenize<T>(Lexer lexer) where T : ITokenizationStrategy
	{
		return T.TryTokenize(lexer);
	}
}