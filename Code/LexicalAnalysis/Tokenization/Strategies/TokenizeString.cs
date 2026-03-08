

namespace LexicalAnalysis.Tokenization.Strategies;

public class TokenizeString : ITokenizerStrategy
{
	public bool IsTokenizable(LexicalAnalyzer a)
	{
		return a.CurrentChar == '"';
	}

	public void Tokenize(LexicalAnalyzer a)
	{
		string str = "";
		int startCol = a.CurrentColumn;
		a.CurrentPosition++; // Skip opening quote
		a.CurrentColumn++;

		while (a.CurrentPosition < a.Input.Length && a.Input[a.CurrentPosition] != '"')
		{
			str += a.Input[a.CurrentPosition];
			a.CurrentPosition++;
			a.CurrentColumn++;
		}

		if (a.CurrentPosition < a.Input.Length && a.Input[a.CurrentPosition] == '"')
		{
			a.CurrentPosition++; // Skip closing quote
			a.CurrentColumn++;
		}

		a.Tokens.Add(new Token(TokenType.String, str, a.CurrentLine, startCol));
	}
}