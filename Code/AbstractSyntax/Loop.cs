namespace AbstractSyntax;

public class Loop
{
	public Melody TheMelody { get; }
	public float StartBeat { get; }
	public float EndBeat { get; }
	public float LengthInBeats => EndBeat - StartBeat;

	public Loop(Melody melody, float startBeat, float endBeat)
	{
		TheMelody = melody;
		StartBeat = startBeat;
		EndBeat = endBeat;
	}


}
