

namespace LexicalAnalysis.Tokenizers;

public class IntegerTokenizer : Tokenizer
{
	protected override bool IsTokenizable(LexicalAnalyzer a)
	{
		return char.IsDigit(a.CursorChar());
	}

	protected override void Tokenize(LexicalAnalyzer a)
	{
		string integer = "";
		int startColumn = a.CursorColumn;

		bool isDigit() => char.IsDigit(a.CursorChar());
		bool isDecimalSymbol() => a.CursorChar() == '\'' || a.CursorChar() == '"';

		while (a.IsNotEndOfFile() && isDigit())
		{
			if (isDecimalSymbol())
			{
				throw new Exception($"Encountered decimal symbols while tokenizing integer at Line: {a.CursorLine}, Column: {a.CursorColumn}");
			}

			integer += a.CursorChar();
			a.AdvanceCursorToNextColumn();
		}

		a.Tokens.Add(new Token(TokenType.Integer, integer, a.CursorLine, startColumn));
	}
}