using AST;

namespace Evaluation;

public class Evaluator
{
	public void Evaluate(Song song)
	{
		PrintToConsole(song); // TODO: Remove after debugging.
		AudioRenderer.Render(song);
	}

	private void PrintToConsole(Song song) // TODO: Remove after debugging. It's just an example.
	{
		foreach (Pattern pattern in song.Patterns)
		{
			Console.WriteLine($"\n=== Pattern: {pattern.Id} ===");
			Console.WriteLine($"Length: {pattern.Length}");

			Console.WriteLine("Samples:");
			foreach (string sampleId in pattern.SampleIds)
			{
				Sample sample = song.GetSample(sampleId);
				Console.WriteLine($"  - {sample.Id} = '{sample.FilePath}', reference note octave = {sample.ReferencePitch.Octave}");
			}

			Console.WriteLine("Notes:");
			foreach (Note note in pattern.Notes)
			{
				Console.WriteLine($"  - Time: {note.StartTime:D2}-{note.EndTime:D2}, Octave: {note.Pitch.Octave}, Pitch class: {note.Pitch.PitchClass}");
			}
		}
	}
}