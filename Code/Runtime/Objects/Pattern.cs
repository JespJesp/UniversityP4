namespace Runtime.Objects;

public class Pattern : IRuntimeObject
{
	public float LengthInBeats;
	public List<Pattern> Patterns = new();
	public List<Melody> Melodies = new();
}