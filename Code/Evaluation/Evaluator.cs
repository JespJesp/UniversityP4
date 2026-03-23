using AbstractSyntax;

namespace Evaluation;

public class Evaluator
{
	public void Evaluate()
	{
		PrintToConsole(); // TODO: Remove after debugging.
		AudioRenderer.Render();
	}

	private void PrintToConsole() // TODO: Remove after debugging. It's just an example.
	{
		foreach (Melody melody in AST.Melodies.Values)
		{
			Console.WriteLine($"\n=== Melody: {melody.Id} ===");
			Console.WriteLine($"Length: {melody.Length}");

			Console.WriteLine("Samples:");
			foreach (string sampleId in melody.SampleIds)
			{
				Sample sample = AST.Samples[sampleId];
				Console.WriteLine($"  - {sample.Id} = '{sample.FilePath}', reference note octave = {sample.ReferencePitch.Octave}");
			}

			Console.WriteLine("Notes:");
			foreach (Note note in melody.Notes)
			{
				Console.WriteLine($"  - Time: {note.StartTime:D2}-{note.EndTime:D2}, Octave: {note.Pitch.Octave}, Pitch class: {note.Pitch.PitchClass}");
			}
		}
	}
}