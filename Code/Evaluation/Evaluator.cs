using AbstractSyntax;

namespace Evaluation;

public class Evaluator
{
	public void Evaluate()
	{
		// TODO: Remove; it's for debugging
		PrintToConsole();

		RuntimeEnvironment.TheTimeline.BuildLoopsFromCommands();

		AudioRenderer.Render();
	}

	private void PrintToConsole() // TODO: Remove after debugging. It's just an example.
	{
		foreach (Pattern pattern in RuntimeEnvironment.Patterns.Values)
		{
			Console.WriteLine($"\n=== Pattern: {pattern.Id} ===");
			Console.WriteLine($"Length: {pattern.LengthInBeats}");

			Console.WriteLine("Children:");
			foreach (string patternAndMelodyIds in pattern.PatternAndMelodyIds)
			{
				Console.WriteLine($"  - {patternAndMelodyIds}");
			}
		}

		foreach (Melody melody in RuntimeEnvironment.Melodies.Values)
		{
			Console.WriteLine($"\n=== Melody: {melody.Id} ===");
			Console.WriteLine($"Length: {melody.LengthInBeats}");

			Console.WriteLine("Samples:");
			foreach (string sampleId in melody.SampleIds)
			{
				Sample sample = RuntimeEnvironment.Samples[sampleId];
				Console.WriteLine($"  - {sample.Id} = '{sample.FilePath}', reference note octave = {sample.ReferencePitch.Octave}");
			}

			Console.WriteLine("Notes:");
			foreach (Note note in melody.Notes)
			{
				Console.WriteLine($"  - Time: {note.StartBeat}-{note.EndBeat}, Octave: {note.ThePitch.Octave}, Pitch class: {note.ThePitch.PitchClass}");
			}
		}
	}
}