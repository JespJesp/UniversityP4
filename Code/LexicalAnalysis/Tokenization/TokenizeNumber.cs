

namespace LexicalAnalysis.Tokenization;

public class TokenizeNumber : Tokenizer
{
	protected override bool IsTokenizable(LexicalAnalyzer a)
	{
		return char.IsDigit(a.CurrentChar);
	}

	protected override void Tokenize(LexicalAnalyzer a)
	{
		string number = "";
		int startColumn = a.CurrentColumn;

		bool isNotEndOfFile() => a.CurrentPosition < a.Input.Length;
		bool isDigit() => char.IsDigit(a.Input[a.CurrentPosition]);

		while (isNotEndOfFile() && isDigit())
		{
			number += a.Input[a.CurrentPosition];
			a.AdvancePosition();
		}

		a.Tokens.Add(new Token(TokenType.Number, number, a.CurrentLine, startColumn));
	}
}