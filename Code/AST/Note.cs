namespace AST;

public class Note
{
	public Pattern ParentPattern;
	public int StartTime;
	public int EndTime;
	public string Pitch = "";
	public float Volume = 1.0f;
	public IExpression? PitchExpression;

	public float Pan = 0.0f;
	public Note(Pattern parentPattern)
	{
		ParentPattern = parentPattern;
	}
}