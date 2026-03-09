

namespace LexicalAnalysis.Tokenizers;

public class WhitespaceTokenizer : Tokenizer
{
	protected override bool IsTokenizable(LexicalAnalyzer a)
	{
		return char.IsWhiteSpace(a.CursorChar());
	}

	protected override void Tokenize(LexicalAnalyzer a)
	{
		if (a.CursorChar() == '\n')
		{
			a.Tokens.Add(new Token(TokenType.NewLine, "\\n", a.CursorLine, a.CursorColumn));
			a.AdvanceCursorToNewLine();
		}
		else if (a.CursorChar() == '\t')
		{
			a.Tokens.Add(new Token(TokenType.Tab, "\\t", a.CursorLine, a.CursorColumn));
			a.AdvanceCursorToNextColumn();
		}
		else // Ignore whitespace if not a tab or a new line
		{
			a.AdvanceCursorToNextColumn();
		}
	}
}