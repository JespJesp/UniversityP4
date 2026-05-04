namespace Runtime.Objects;

public class Note : IRuntimeObject
{
	public float StartBeat;
	public float EndBeat;
	public Pitch Pitch = new();
	public float Volume = 1.0f;
	public float Pan = 0.0f;
	public Sample? SampleOverride = null;
}