using JespAst.Nodes.Melodies;

namespace JespRuntime.Nodes;

public class Melody
{
	public float LengthInBeats;
	public List<Sample> Samples = new();
	public List<Note> Notes = new();
}