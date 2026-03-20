

namespace LexicalAnalysis;

public enum TokenType
{
	// Identifier and keywords
	Identifier,
	TimelineKeyword,
	SamplesKeyword,
	NotesKeyword,
	GainKeyword,

	// Values
	Integer,
	String,

	// Formatting
	Hyphen,
	NewLine,
	Tab,
	EndOfFile,
}