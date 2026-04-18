namespace Tokens;

public enum TokenType
{
	// Keywords
	TimelineKeyword,
	SampleKeyword,
	SamplesKeyword,
	ChordsKeyword,
	MelodyKeyword,
	PatternKeyword,
	GainKeyword,
	PanKeyword,
	StringKeyword,
	FloatKeyword,

	// Values
	Identifier,
	Integer,
	Float,
	String,

	// Formatting
	LeftParentheses,
	RightParentheses,
	ForwardSlash,
	Comma,
	Plus,
	Asterisk,
	Slash,
	Newline,
	Indent,
	EndOfFile
}

