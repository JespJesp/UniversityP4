using Visitation;
using Song;

namespace Evaluation;

public class Printer : IVisitor
{
	private Pattern _currentPattern;

	public void Evaluate(Pattern pattern)
	{
		_currentPattern = pattern;
		pattern.Accept(this);
	}

	public void VisitPattern(Pattern pattern)
	{
		Console.WriteLine($"\n=== Pattern: {pattern.Name} ===");
		Console.WriteLine($"Length: {pattern.Length}");

		// Visit all samples first (as requested in the requirement)
		Console.WriteLine("Samples:");
		foreach (var sample in pattern.Samples)
			sample.Accept(this);

		// Then visit notes
		Console.WriteLine("Notes:");
		foreach (var note in pattern.Notes)
			note.Accept(this);
	}

	public void VisitSample(Sample sample)
	{
		Console.WriteLine($"  - {sample.FileName}");
	}

	public void VisitNote(Note note)
	{
		Console.WriteLine($"  Time: {note.StartTime:D2}-{note.EndTime:D2}, Pitch: {note.Pitch}");
	}
}