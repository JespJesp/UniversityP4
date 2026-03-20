namespace LexicalAnalysis;

public class LexicalAnalyzerCursor
{
	/// <summary>
	/// This is the index position in the text.
	/// </summary>
	public int Position { get; private set; } // Private set because the "MoveX" methods handle set
	public int Line { get; private set; } // Private set because the "MoveX" methods handle set
	public int Column { get; private set; } // Private set because the "MoveX" methods handle set

	public void MoveToStartPosition()
	{
		Line = 1;
		Column = 1;
		Position = 0;
	}

	public void MoveToNextColumn()
	{
		Position++;
		Column++;
	}

	public void MoveoToNewLine()
	{
		Position++;
		Line++;
		Column = 1;
	}
}