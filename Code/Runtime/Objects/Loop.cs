namespace Runtime.Objects;

public class Loop : RuntimeObject
{
	public Melody Melody0 { get; }
	public float StartBeat { get; }
	public float EndBeat { get; }

	public float LengthInBeats => EndBeat - StartBeat;

	public Loop(Melody melody, float startBeat, float endBeat)
	{
		Melody0 = melody;
		StartBeat = startBeat;
		EndBeat = endBeat;
	}
}
