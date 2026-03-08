

namespace LexicalAnalysis.Tokenization;

public class TokenizeString : Tokenizer
{
	protected override bool IsTokenizable(LexicalAnalyzer a)
	{
		return a.CurrentChar == '"';
	}

	protected override void Tokenize(LexicalAnalyzer a)
	{
		string str = "";
		int startColumn = a.CurrentColumn;
		a.AdvancePosition(); // Skip opening quote

		bool isNotEndOfFile() => a.CurrentPosition < a.Input.Length;
		bool isClosingQuote() => a.Input[a.CurrentPosition] == '"';

		while (isNotEndOfFile() && !isClosingQuote())
		{
			str += a.Input[a.CurrentPosition];
			a.AdvancePosition();
		}

		if (!isNotEndOfFile() && !isClosingQuote())
		{
			throw new Exception(
				$"Error while tokenizing string: Missing closing quote '\"' at Line:{a.CurrentLine} Column:{a.CurrentColumn}");
		}

		a.AdvancePosition(); // Skip closing quote
		a.Tokens.Add(new Token(TokenType.String, str, a.CurrentLine, startColumn));
	}
}