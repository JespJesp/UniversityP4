namespace AST;

public class Pattern
{
	public int Length;
	public string Name = "";
	public string? ScaleName;
	public List<Sample> Samples = new List<Sample>();
	public List<Note> Notes = new List<Note>();
}