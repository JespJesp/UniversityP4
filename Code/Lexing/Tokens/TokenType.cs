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

	// Values
	Integer,
	Float,
	String,

	// Formatting
	LeftParentheses,
	RightParentheses,
	ForwardSlash,
	Comma,
	Newline,
	Indent,
	EndOfFile
}

