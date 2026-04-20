using Phases.Lexing;
using Tokens.Tokenization.Strategies;

namespace Tokens.Tokenization;

public static class Tokenizer
{
	public static bool TryTokenize<T>(Lexer lexer) where T : ITokenizationStrategy
	{
		return T.TryTokenize(lexer);
	}
}