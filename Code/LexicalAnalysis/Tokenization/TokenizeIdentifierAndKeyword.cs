

namespace LexicalAnalysis.Tokenization;

public class TokenizeIdentifierAndKeyword : Tokenizer
{
	protected override bool IsTokenizable(LexicalAnalyzer a)
	{
		return char.IsLetter(a.CurrentChar) || a.CurrentChar == '_';
	}

	protected override void Tokenize(LexicalAnalyzer a)
	{
		string identifier = "";
		int startColumn = a.CurrentColumn;

		bool isNotEndOfFile() => a.CurrentPosition < a.Input.Length;
		bool isLetterOrDigit() => char.IsLetterOrDigit(a.Input[a.CurrentPosition]) || a.Input[a.CurrentPosition] == '_';

		while (isNotEndOfFile() && isLetterOrDigit())
		{
			identifier += a.Input[a.CurrentPosition];
			a.AdvancePosition();
		}

		TokenType tokenType = identifier switch
		{
			"samples" => TokenType.SamplesKeyword,
			"notes" => TokenType.NotesKeyword,
			_ => TokenType.Identifier
		};

		a.Tokens.Add(new Token(tokenType, identifier, a.CurrentLine, startColumn));
	}
}