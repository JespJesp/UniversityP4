

namespace LexicalAnalysis.Tokenizers;

public class StringTokenizer : Tokenizer
{
	protected override bool IsTokenizable(LexicalAnalyzer a)
	{
		return a.CursorChar() == '"';
	}

	protected override void Tokenize(LexicalAnalyzer a)
	{
		string str = "";
		int startColumn = a.CursorColumn;
		a.AdvanceCursorToNextColumn(); // Skip opening quote

		bool isClosingQuote() => a.CursorChar() == '"';

		while (!isClosingQuote())
		{
			str += a.CursorChar();
			a.AdvanceCursorToNextColumn();

			if (!a.IsNotEndOfFile())
			{
				throw new Exception(
					$"String is missing closing quote '\"' at Line:{a.CursorLine} Column:{a.CursorColumn}");
			}
		}

		a.AdvanceCursorToNextColumn(); // Skip closing quote
		a.Tokens.Add(new Token(TokenType.String, str, a.CursorLine, startColumn));
	}
}