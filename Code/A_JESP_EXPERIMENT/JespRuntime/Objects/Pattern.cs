using JespAst.Nodes.Patterns;

namespace JespRuntime.Objects;

public class Pattern : RuntimeObject
{
	public float LengthInBeats;
	public List<Pattern> Patterns = new();
	public List<Melody> Melodies = new();
}