

namespace LexicalAnalysis.Tokenization;

public class TokenizeIdentifierOrKeyword : Tokenizer
{
	protected override bool IsTokenizable(LexicalAnalyzer a)
	{
		return char.IsLetter(a.CursorChar()) || a.CursorChar() == '_';
	}

	protected override void Tokenize(LexicalAnalyzer a)
	{
		string identifier = "";
		int startColumn = a.CursorColumn;

		bool isNotEndOfFile() => a.CursorPosition < a.Input.Length;
		bool isLetterOrDigit() => char.IsLetterOrDigit(a.CursorChar()) || a.CursorChar() == '_';

		while (isNotEndOfFile() && isLetterOrDigit())
		{
			identifier += a.CursorChar();
			a.AdvanceCursor();
		}

		TokenType tokenType = identifier switch
		{
			"samples" => TokenType.SamplesKeyword,
			"notes" => TokenType.NotesKeyword,
			_ => TokenType.Identifier
		};

		a.Tokens.Add(new Token(tokenType, identifier, a.CursorLine, startColumn));
	}
}