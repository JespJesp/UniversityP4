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
	NewLine,
	Tab,
	EndOfFile,
}