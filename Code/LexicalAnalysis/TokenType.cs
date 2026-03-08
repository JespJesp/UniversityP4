

namespace LexicalAnalysis;

public enum TokenType
{
	Identifier,

	SamplesKeyword,
	NotesKeyword,

	Number,
	String,

	Hyphen,
	NewLine,
	EndOfFile,
	Unknown
}