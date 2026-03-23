namespace AbstractSyntax;

public class Note
{
	public Melody ParentMelody;
	public float StartBeat;
	public float EndBeat;
	public Pitch ThePitch = new("c5"); // Default value

	public Note(Melody parentMelody)
	{
		ParentMelody = parentMelody;
	}
}