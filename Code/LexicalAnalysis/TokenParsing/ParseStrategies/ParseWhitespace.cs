

namespace LexicalAnalysis.TokenParsing.ParseStrategies;

public class ParseWhitespace : ITokenParseStrategy
{
	public bool IsParsable(TokenizationOperation op)
	{
		return char.IsWhiteSpace(op.CurrentChar);
	}

	public void Parse(TokenizationOperation op)
	{
		if (op.CurrentChar == '\n')
		{
			op.Tokens.Add(new Token(TokenType.NewLine, "\n", op.CurrentLine, op.CurrentColumn));
			op.CurrentLine++;
			op.CurrentColumn = 1;
		}
		else
		{
			op.CurrentColumn++;
		}
		op.CurrentPosition++;
	}
}