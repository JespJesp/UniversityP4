namespace Runtime.Objects;

public class Melody : IRuntimeObject
{
	public float LengthInBeats;
	public List<Sample> Samples = new();
	public List<Note> Notes = new();
}