using Visitation;

namespace Song;

public class Note : IVisitable
{
	public int StartTime { get; set; }
	public int EndTime { get; set; }
	public string Pitch { get; set; }

	public void Accept(IVisitor visitor)
	{
		visitor.VisitNote(this);
	}
}