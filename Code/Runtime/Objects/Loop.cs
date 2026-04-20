namespace Runtime.Objects;

public class Loop : RuntimeObject
{
	public Melody Melody0 = new();
	public float StartBeat;
	public float EndBeat;

	public float LengthInBeats => EndBeat - StartBeat;
}
