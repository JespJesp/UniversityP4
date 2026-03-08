

namespace LexicalAnalysis.TokenParsing.ParseStrategies;

public class ParseString : ITokenParseStrategy
{
	public bool IsParsable(TokenizationOperation op)
	{
		return op.CurrentChar == '"';
	}

	public void Parse(TokenizationOperation op)
	{
		string str = "";
		int startCol = op.CurrentColumn;
		op.CurrentPosition++; // Skip opening quote
		op.CurrentColumn++;

		while (op.CurrentPosition < op.Input.Length && op.Input[op.CurrentPosition] != '"')
		{
			str += op.Input[op.CurrentPosition];
			op.CurrentPosition++;
			op.CurrentColumn++;
		}

		if (op.CurrentPosition < op.Input.Length && op.Input[op.CurrentPosition] == '"')
		{
			op.CurrentPosition++; // Skip closing quote
			op.CurrentColumn++;
		}

		op.Tokens.Add(new Token(TokenType.String, str, op.CurrentLine, startCol));
	}
}