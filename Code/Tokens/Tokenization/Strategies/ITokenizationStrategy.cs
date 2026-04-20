using Phases.Lexing;

namespace Tokens.Tokenization.Strategies;

public interface ITokenizationStrategy
{
	public static abstract bool TryTokenize(Lexer lexer);
}