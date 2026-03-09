

namespace LexicalAnalysis;

public enum TokenType
{
	// Identifiers and keywords
	Identifier,
	KeywordTimeline,
	KeywordSamples,
	KeywordNotes,

	// Values
	Integer,
	String,

	// Formatting
	Hyphen,
	NewLine,
	Tab,
	EndOfFile,
}