

namespace LexicalAnalysis.Tokenization.Strategies;

public class TokenizeNumber : ITokenizerStrategy
{
	public bool IsTokenizable(LexicalAnalyzer a)
	{
		return char.IsDigit(a.CurrentChar);
	}

	public void Tokenize(LexicalAnalyzer a)
	{
		string number = "";
		int startCol = a.CurrentColumn;
		while (a.CurrentPosition < a.Input.Length && char.IsDigit(a.Input[a.CurrentPosition]))
		{
			number += a.Input[a.CurrentPosition];
			a.CurrentPosition++;
			a.CurrentColumn++;
		}

		a.Tokens.Add(new Token(TokenType.Number, number, a.CurrentLine, startCol));
	}
}