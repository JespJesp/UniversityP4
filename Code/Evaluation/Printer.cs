using AST;

namespace Evaluation;

public class Printer
{
	public void Evaluate(Song song)
	{
		foreach (Pattern pattern in song.Patterns)
		{
			Console.WriteLine($"\n=== Pattern: {pattern.Name} ===");
			Console.WriteLine($"Length: {pattern.Length}");

			Console.WriteLine("Samples:");
			foreach (Sample sample in pattern.Samples)
			{
				Console.WriteLine($"  - {sample.FileName}");
			}

			Console.WriteLine("Notes:");
			foreach (Note note in pattern.Notes)
			{
				Console.WriteLine($"  Time: {note.StartTime:D2}-{note.EndTime:D2}, Pitch: {note.Pitch}");
			}
		}
	}
}