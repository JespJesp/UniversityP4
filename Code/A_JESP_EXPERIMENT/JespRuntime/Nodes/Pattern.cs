using JespAst.Nodes.Patterns;

namespace JespRuntime.Nodes;

public class Pattern
{
	public float LengthInBeats;
	public List<Pattern> Patterns = new();
	public List<Melody> Melodies = new();
}