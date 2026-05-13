using Phases.Lexing;
using Tokens.Tokenization.Strategies;

namespace Tokens.Tokenization;

public static class Tokenizer
{
	public static bool TryTokenize(FileLexer lexer)
	{
		return WhitespaceStrategy.TryTokenize(lexer)
				|| ImportStrategy.TryTokenize(lexer)
				|| CommentStrategy.TryTokenize(lexer)
				|| StringStrategy.TryTokenize(lexer)
				|| LeftParenthesesStrategy.TryTokenize(lexer)
				|| RightParenthesesStrategy.TryTokenize(lexer)
				|| CommaStrategy.TryTokenize(lexer)
				|| PlusStrategy.TryTokenize(lexer)
				|| AsteriskStrategy.TryTokenize(lexer)
				|| SlashStrategy.TryTokenize(lexer)
				|| NumberOrMinusStrategy.TryTokenize(lexer)
				|| IdentifierStrategy.TryTokenize(lexer);
	}
}