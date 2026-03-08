using Visitation;

namespace Song;

public class Sample : IVisitable
{
	public string FileName { get; set; }

	public void Accept(IVisitor visitor)
	{
		visitor.VisitSample(this);
	}
}