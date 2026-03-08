

namespace Visitation;

public interface IVisitable
{
	public void Accept(IVisitor visitor);
}