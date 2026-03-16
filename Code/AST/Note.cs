namespace AST;

public class Note
{
	public Pattern ParentPattern;
	public int StartTime;
	public int EndTime;
	public Pitch Pitch = new("c5");

	public Note(Pattern parentPattern)
	{
		ParentPattern = parentPattern;
	}
}