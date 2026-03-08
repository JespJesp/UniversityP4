

namespace LexicalAnalysis.TokenParsing.ParseStrategies;

public class ParseUnknownCharacter : ITokenParseStrategy
{
	public bool IsParsable(TokenizationOperation op)
	{
		return true; // Unknown characters are always parsed
	}

	public void Parse(TokenizationOperation op)
	{
		op.Tokens.Add(new Token(TokenType.Unknown, op.CurrentChar.ToString(), op.CurrentLine, op.CurrentColumn));
		op.CurrentPosition++;
		op.CurrentColumn++;
	}
}