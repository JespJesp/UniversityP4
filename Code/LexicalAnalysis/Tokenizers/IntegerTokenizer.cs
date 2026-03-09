

namespace LexicalAnalysis.Tokenizers;

public class IntegerTokenizer : Tokenizer
{
	protected override bool IsTokenizable(LexicalAnalyzer a)
	{
		return char.IsDigit(a.CursorChar());
	}

	protected override void Tokenize(LexicalAnalyzer a)
	{
		string number = "";
		int startColumn = a.CursorColumn;

		bool isNotEndOfFile() => a.CursorPosition < a.InputText.Length;
		bool isDigit() => char.IsDigit(a.CursorChar());
		bool isNotDecimal() => a.CursorChar() != '\'' || a.CursorChar() != '"';

		while (isNotEndOfFile() && isDigit())
		{
			if (isNotDecimal())
			{
				throw new Exception("Error while tokenizing integer: Encountered decimal symbols.");
			}

			number += a.CursorChar();
			a.AdvanceCursor();
		}

		a.Tokens.Add(new Token(TokenType.Integer, number, a.CursorLine, startColumn));
	}
}