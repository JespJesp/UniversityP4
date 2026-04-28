namespace Runtime.Objects;

public class Melody : RuntimeObject
{
	public float LengthInBeats;
	public List<Sample> Samples = new();
	public List<Note> Notes = new();
	public Scale? Scale = null;	
}