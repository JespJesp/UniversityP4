

namespace LexicalAnalysis.TokenParsing;

public interface ITokenParseStrategy
{
	public bool IsParsable(TokenizationOperation operation);
	public void Parse(TokenizationOperation operation);
}