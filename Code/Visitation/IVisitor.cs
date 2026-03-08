using Song;

namespace Visitation;

public interface IVisitor
{
	void VisitPattern(Pattern pattern);
	void VisitSample(Sample sample);
	void VisitNote(Note note);
}