namespace JespRuntime.Nodes;

public class Note
{
	public float StartBeat;
	public float EndBeat;
	public Pitch Pitch0 = new("c5"); // Default

	public float Volume = 1.0f;
	public float Pan = 0.0f;
}