

namespace LexicalAnalysis.Tokenizers;

public class IdentifierOrKeywordTokenizer : Tokenizer
{
	protected override bool IsTokenizable(LexicalAnalyzer a)
	{
		return char.IsLetter(a.CursorChar()) || a.CursorChar() == '_';
	}

	protected override void Tokenize(LexicalAnalyzer a)
	{
		string identifier = "";
		int startColumn = a.CursorColumn;

		bool isNotEndOfFile() => a.CursorPosition < a.InputText.Length;
		bool isLetterOrDigit() => char.IsLetterOrDigit(a.CursorChar()) || a.CursorChar() == '_';

		while (isNotEndOfFile() && isLetterOrDigit())
		{
			identifier += a.CursorChar();
			a.AdvanceCursor();
		}

		TokenType tokenType = identifier switch
		{
			"timeline" => TokenType.KeywordTimeline,
			"samples" => TokenType.KeywordSamples,
			"notes" => TokenType.KeywordNotes,
			_ => TokenType.Identifier // Underscore encompasses all other strings
		};

		a.Tokens.Add(new Token(tokenType, identifier, a.CursorLine, startColumn));
	}
}