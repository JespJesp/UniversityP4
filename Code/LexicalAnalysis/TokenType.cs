

namespace LexicalAnalysis;

public enum TokenType
{
	// Identifier and keywords
	Identifier,
	TimelineKeyword,
	SamplesKeyword,
	NotesKeyword,
	ScaleKeyword,


	// Values
	Integer,
	String,

	// Formatting
	Hyphen,
	Comma,
	NewLine,
	Tab,
	EndOfFile,
}