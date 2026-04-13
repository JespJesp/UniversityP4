namespace Lexing.Tokens;

public enum TokenType
{
	// Identifier and keywords
	Identifier,
	TimelineKeyword,
	SampleKeyword,
	SamplesKeyword,
	ChordsKeyword,
	MelodyKeyword,
	PatternKeyword,
	GainKeyword,
	PanKeyword,
	StringKeyword,

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

