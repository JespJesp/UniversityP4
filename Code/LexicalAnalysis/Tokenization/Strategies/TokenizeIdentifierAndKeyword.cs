

namespace LexicalAnalysis.Tokenization.Strategies;

public class TokenizeIdentifierAndKeyword : ITokenizerStrategy
{
	public bool IsTokenizable(LexicalAnalyzer a)
	{
		return char.IsLetter(a.CurrentChar) || a.CurrentChar == '_';
	}

	public void Tokenize(LexicalAnalyzer a)
	{
		string identifier = "";
		int startCol = a.CurrentColumn;
		while (a.CurrentPosition < a.Input.Length && (char.IsLetterOrDigit(a.Input[a.CurrentPosition]) || a.Input[a.CurrentPosition] == '_'))
		{
			identifier += a.Input[a.CurrentPosition];
			a.CurrentPosition++;
			a.CurrentColumn++;
		}

		TokenType type = identifier switch
		{
			"samples" => TokenType.Sample,
			"notes" => TokenType.Note,
			_ => TokenType.Identifier
		};

		a.Tokens.Add(new Token(type, identifier, a.CurrentLine, startCol));
	}
}