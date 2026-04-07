namespace Runtime;

public class Note
{
	public Melody ParentMelody;
	public float StartBeat;
	public float EndBeat;
	public Pitch ThePitch = new("c5"); // Default
	public float Volume = 1.0f;
	public float Pan = 0.0f;

	public Note(Melody parentMelody)
	{
		ParentMelody = parentMelody;
	}
}