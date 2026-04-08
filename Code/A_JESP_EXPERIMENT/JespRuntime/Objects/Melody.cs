using JespAst.Nodes.Melodies;

namespace JespRuntime.Objects;

public class Melody : RuntimeObject
{
	public float LengthInBeats;
	public List<Sample> Samples = new();
	public List<Note> Notes = new();
}