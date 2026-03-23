namespace AST;

public class Note
{
	public Pattern ParentPattern;
	public int StartTime;
	public int EndTime;
	public string Pitch = "";
	public IExpression? PitchExpression;

	public Note(Pattern parentPattern)
	{
		ParentPattern = parentPattern;
	}
}