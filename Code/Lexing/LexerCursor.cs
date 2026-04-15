namespace Lexing;

public class LexerCursor
{
	/// <summary>
	/// This is the index position in the text.
	/// </summary>
	public int Position { get; private set; } = 0;
	public int Line { get; private set; } = 1;
	public int Column { get; private set; } = 1;

	public void MoveToNextColumn()
	{
		Position++;
		Column++;
	}

	public void MoveToNewLine()
	{
		Position++;
		Line++;
		Column = 1;
	}
}