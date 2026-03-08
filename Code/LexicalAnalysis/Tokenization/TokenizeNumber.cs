

namespace LexicalAnalysis.Tokenization;

public class TokenizeNumber : Tokenizer
{
	protected override bool IsTokenizable(LexicalAnalyzer a)
	{
		return char.IsDigit(a.CursorChar());
	}

	protected override void Tokenize(LexicalAnalyzer a)
	{
		string number = "";
		int startColumn = a.CursorColumn;

		bool isNotEndOfFile() => a.CursorPosition < a.Input.Length;
		bool isDigit() => char.IsDigit(a.CursorChar());

		while (isNotEndOfFile() && isDigit())
		{
			number += a.CursorChar();
			a.AdvanceCursor();
		}

		a.Tokens.Add(new Token(TokenType.Number, number, a.CursorLine, startColumn));
	}
}