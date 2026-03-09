

namespace LexicalAnalysis.Tokenizers;

public class WhitespaceTokenizer : Tokenizer
{
	const int TabWidth = 3;

	protected override bool IsTokenizable(LexicalAnalyzer a)
	{
		return char.IsWhiteSpace(a.CursorChar());
	}

	protected override void Tokenize(LexicalAnalyzer a)
	{
		if (a.CursorChar() == '\n')
		{
			a.Tokens.Add(new Token(TokenType.NewLine, "\n", a.CursorLine, a.CursorColumn));
			a.AdvanceCursorToNewLine();
			return;
		}

		bool isNotEndOfFile() => a.CursorPosition < a.InputText.Length;
		bool isWhitespace() => char.IsWhiteSpace(a.CursorChar());
		int whitespaceWidth = 1;

		// Whitespace not wide enough to be considered a tab is skipped
		while (isNotEndOfFile() && isWhitespace())
		{
			if (whitespaceWidth == TabWidth)
			{
				a.Tokens.Add(new Token(TokenType.Tab, "", a.CursorLine, a.CursorColumn));
				a.AdvanceCursor();
				break;
			}

			a.AdvanceCursor();
			whitespaceWidth++;
		}
	}
}