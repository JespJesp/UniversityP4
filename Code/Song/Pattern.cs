using Visitation;

namespace Song;

public class Pattern : IVisitable
{
	public int Length { get; set; }
	public string Name { get; set; }
	public List<Sample> Samples { get; set; } = new List<Sample>();
	public List<Note> Notes { get; set; } = new List<Note>();

	public void Accept(IVisitor visitor)
	{
		visitor.VisitPattern(this);
	}
}