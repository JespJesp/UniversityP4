namespace LexicalAnalysis;

public enum TokenType
{
	// Identifier and keywords
	Identifier,
	TimelineKeyword,
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
	ForwardSlash,
	NewLine,
	Tab,
	EndOfFile,
}