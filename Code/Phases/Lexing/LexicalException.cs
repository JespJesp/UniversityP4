namespace Phases.Lexing;

public class LexicalException : Exception
{
	public LexicalException(int line, int column, string message)
		: base($"Line: {line}. Column: {column}. {message}") { }
}