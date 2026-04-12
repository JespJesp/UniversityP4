namespace Lexing;

public record LexicalError
(
	int Line,
	int Column,
	string Message
);
