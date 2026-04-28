namespace Phases.Lexing;

public class LexicalError : Exception
{
	public int Line;
	public int Column;

	public LexicalError(int line, int column, string message) : base(message)
	{
		Line = line;
		Column = column;
	}
}