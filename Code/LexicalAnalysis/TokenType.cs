namespace LexicalAnalysis;

public enum TokenType
{
	// Identifier and keywords
	Identifier,
	TimelineKeyword,
	SamplesKeyword,
	NotesKeyword,

	// Values
	Integer,
	Float,
	String,

	// Formatting
	Hyphen,
	LeftParen,
	RightParen,
	Comma,
	NewLine,
	Tab,
	EndOfFile,
}