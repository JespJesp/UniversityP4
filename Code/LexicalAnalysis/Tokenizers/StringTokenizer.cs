

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
		a.AdvanceCursor(); // Skip opening quote

		bool isNotEndOfFile() => a.CursorPosition < a.InputText.Length;
		bool isClosingQuote() => a.CursorChar() == '"';

		while (isNotEndOfFile() && !isClosingQuote())
		{
			str += a.CursorChar();
			a.AdvanceCursor();
		}

		if (!isNotEndOfFile() && !isClosingQuote())
		{
			throw new Exception(
				$"Error while tokenizing string: Missing closing quote '\"' at Line:{a.CursorLine} Column:{a.CursorColumn}");
		}

		a.AdvanceCursor(); // Skip closing quote
		a.Tokens.Add(new Token(TokenType.String, str, a.CursorLine, startColumn));
	}
}