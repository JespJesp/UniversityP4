

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
	Hyphen,
	Comma,
	NewLine,
	Tab,
	EndOfFile,
}