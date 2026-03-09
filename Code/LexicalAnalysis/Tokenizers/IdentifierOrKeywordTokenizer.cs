

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

		while (a.IsNotEndOfFile() && char.IsLetterOrDigit(a.CursorChar()) || a.CursorChar() == '_')
		{
			identifier += a.CursorChar();
			a.AdvanceCursorToNextColumn();
		}

		TokenType tokenType = identifier switch
		{
			"timeline" => TokenType.TimelineKeyword,
			"samples" => TokenType.SamplesKeyword,
			"notes" => TokenType.NotesKeyword,
			_ => TokenType.Identifier // The underscore notation encompasses all other strings
		};

		a.Tokens.Add(new Token(tokenType, identifier, a.CursorLine, startColumn));
	}
}