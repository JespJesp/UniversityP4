

namespace LexicalAnalysis.TokenParsing;

public static class TokenParser
{
	public static bool TryParse(ITokenParseStrategy strategy, TokenizationOperation operation)
	{
		if (strategy.IsParsable(operation))
		{
			strategy.Parse(operation);
			return true;
		}
		return false;
	}
}