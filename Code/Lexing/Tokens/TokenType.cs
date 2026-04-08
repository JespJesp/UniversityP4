namespace Lexing.Tokens;

public enum TokenType
{
	// Identifier and keywords
	Identifier,
	TimelineKeyword,
	SampleKeyword,
	SamplesKeyword,
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

