namespace LexicalAnalysis.Tokens;

public enum TokenType
{
	// Identifier and keywords
	Identifier,
	TimelineKeyword,
	SampleKeyword,
	NotesKeyword,
	MelodyKeyword,
	PatternKeyword,
	GainKeyword,
	PanKeyword,

	// Values
	Integer,
	Float,
	String,

	// Formatting
	LeftParentheses,
	RightParentheses,
	Comma,
	Newline,
	Indent,
	EndOfFile
}

