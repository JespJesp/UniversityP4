

namespace LexicalAnalysis.TokenParsing.ParseStrategies;

public class ParseNumber : ITokenParseStrategy
{
	public bool IsParsable(TokenizationOperation op)
	{
		return char.IsDigit(op.CurrentChar);
	}

	public void Parse(TokenizationOperation op)
	{
		string number = "";
		int startCol = op.CurrentColumn;
		while (op.CurrentPosition < op.Input.Length && char.IsDigit(op.Input[op.CurrentPosition]))
		{
			number += op.Input[op.CurrentPosition];
			op.CurrentPosition++;
			op.CurrentColumn++;
		}

		op.Tokens.Add(new Token(TokenType.Number, number, op.CurrentLine, startCol));
	}
}