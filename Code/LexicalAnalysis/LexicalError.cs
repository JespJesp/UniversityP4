namespace LexicalAnalysis;

public record LexicalError
(
	int Line,
	int Column,
	string Message
);
