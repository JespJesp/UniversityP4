namespace AbstractSyntax;

public class Note
{
	public Melody ParentMelody;
	public int StartTime;
	public int EndTime;
	public Pitch Pitch = new("c5");

	public Note(Melody parentMelody)
	{
		ParentMelody = parentMelody;
	}
}