namespace SyntaxAnalysis;

public class SyntaxAnalyzerCursor
{
	/// <summary>
	/// This is the index position in the list of tokens.
	/// </summary>
	public int Position { get; private set; } // Private set because the "MoveX" methods handle set

	public void MoveToStartPosition()
	{
		Position = 0;
	}

	public void MoveToNextToken()
	{
		Position++;
	}
}