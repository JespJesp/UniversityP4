

namespace LexicalAnalysis.TokenParsing.ParseStrategies;

public class ParseHyphen : ITokenParseStrategy
{
	public bool IsParsable(TokenizationOperation op)
	{
		return op.CurrentChar == '-';
	}

	public void Parse(TokenizationOperation op)
	{
		op.Tokens.Add(new Token(TokenType.Hyphen, "-", op.CurrentLine, op.CurrentColumn));
		op.CurrentPosition++;
		op.CurrentColumn++;
	}
}