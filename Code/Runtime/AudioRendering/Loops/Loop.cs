using Runtime.Objects;

namespace Runtime.AudioRendering.Loops;

public class Loop
{
	public Melody Melody = new();
	public float StartBeat;
	public float EndBeat;

	public float LengthInBeats => EndBeat - StartBeat;
}
